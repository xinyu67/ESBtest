using EsunLikeList.Business;
using EsunLikeList.Common.Models;
using Microsoft.AspNetCore.Mvc;
namespace EsunLikeList.Web.Controllers;
[AutoValidateAntiforgeryToken]
public class LikeListController : Controller
{
    private readonly ILikeListService _svc;
    private readonly string _uid;
    public LikeListController(ILikeListService svc, IConfiguration cfg)
    {
        _svc = svc;
        _uid = cfg["CurrentUserId"]!;
    }
    public async Task<IActionResult> Index(string? keyword)
    {
        ViewBag.User = await _svc.GetUserAsync(_uid);
        ViewBag.Keyword = keyword;
        return View(await _svc.GetListAsync(_uid, keyword));
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var u = await _svc.GetUserAsync(_uid);
        return View(new LikeItemInput { Account = u?.Account ?? "" });
    }
    [HttpPost]
    public async Task<IActionResult> Create(LikeItemInput m)
    {
        if (!ModelState.IsValid) return View(m);
        await _svc.AddAsync(_uid, m);
        TempData["Msg"] = "新增成功";
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var m = await _svc.GetInputAsync(_uid, id);
        return m == null ? NotFound() : View(m);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int id, LikeItemInput m)
    {
        m.SN = id;
        if (!ModelState.IsValid) return View(m);
        if (!await _svc.UpdateAsync(_uid, m)) return NotFound();
        TempData["Msg"] = "更新成功";
        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        TempData["Msg"] = await _svc.DeleteAsync(_uid, id) ? "刪除成功" : "找不到資料";
        return RedirectToAction(nameof(Index));
    }
}
