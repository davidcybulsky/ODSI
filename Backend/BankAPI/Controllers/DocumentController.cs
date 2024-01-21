using BankAPI.Interfaces;
using BankAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankAPI.Controllers;

[ApiController]
[Route("api/document")]
public class DocumentController : ControllerBase
{
    private readonly IDocumentService _service;

    public DocumentController(IDocumentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<DocumentDto>> GetDocumentAsync()
    {
        DocumentDto document = await _service.GetDocumentAsync();
        return Ok(document);
    }
}