using Microsoft.AspNetCore.Mvc;

namespace IPv64IPScanner.Controllers;

[ApiController]
[Route("[controller]")]
public class IPController : ControllerBase
{
    private readonly ILogger<IPController> _logger;

    public IPController(ILogger<IPController> logger)
    {
        _logger = logger;
    }

    [HttpPost("IPList")]
    public async Task<IActionResult> PostIpList([FromBody] List<PostObject> postObjects)
    {
        if (postObjects == null || postObjects.Count == 0)
            return BadRequest(new Response { Message = "Objekt darf nicht leer sein!", Status = 202 });

        foreach (var postObject in postObjects)
        {
            IPList ipList = new IPList();
            ipList.IP_Address = postObject.IP;
            ipList.IP_ExtendedInfos = "";
            ipList.IP_Added = DateTime.UtcNow;
            ipList.IP_Changed = DateTime.UtcNow;
            ipList.IP_Queue = true;
            await ipList.Insert();
        }

        return Ok(new Response { Message = "IP Adressen gespeichert!", Status = 200 });
    }
}