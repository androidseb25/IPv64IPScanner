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

    /// <summary>
    /// Gibt die Liste der IP's zurück
    /// </summary>
    /// <param name="filter">0 = alle; 1 = geblockte; 2 = in der Warteschlange</param>
    /// <returns></returns>
    [HttpGet("List/{filter}")]
    public async Task<IActionResult> GetList(int filter = 0)
    {
        IPList ip = new IPList();

        bool blocked = filter == 1;
        bool queue = filter == 2;
        
        var list = await ip.LoadAll(blocked, queue);

        return Ok(new Response { data = list, Message = "Abfrage erfolgreich!", Status = 200 });
    }

    /// <summary>
    /// Senden einer Liste von IP Adressen die überprüft werden sollen
    /// </summary>
    /// <param name="postObjects">[{IP: "X.X.X.X"}]</param>
    /// <returns></returns>
    [HttpPost("List")]
    public async Task<IActionResult> PostList([FromBody] List<PostObject> postObjects)
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

    /// <summary>
    /// Löscht eine IP aus der Datenbank
    /// </summary>
    /// <param name="id">IP Id in der Datenbank</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (id == 0)
            return BadRequest(new Response { Message = "ID ist 0", Status = 202 });
        
        IPList ip = new IPList();
        bool deleted = await ip.Delete("IP_ID", id);
        
        if (!deleted)
            return BadRequest(new Response { Message = "ID nicht gefunden oder gelöscht!", Status = 404 });
        return Ok(new Response { Message = "IP gelöscht!", Status = 200 });
    }
    
    /// <summary>
    /// Lädt die Blocklistdatei runter
    /// </summary>
    /// <returns>gibt die Blockliste als txt zurück</returns>
    [HttpGet("Blocklist")]
    public async Task<IActionResult> GetBlocklist()
    {
        try
        {
            if (!BlockListCreator.IsExists)
                return BadRequest(new Response { Message = "Datei nicht gefunden!", Status = 404 });
            
            var fileBytes = await System.IO.File.ReadAllBytesAsync(BlockListCreator.GetFilePath);
            var memoryStream = new MemoryStream(fileBytes);

            // Return the file as a FileStreamResult
            return File(memoryStream, "text/plain", "IPv64Blocklist_extended.txt");
        }
        catch (Exception e)
        {
            return BadRequest(new Response { Message = e.Message, Status = 400 });
        }
    }
}