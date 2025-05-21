using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace IPv64IPScanner;

public class MxToolbox
{
    private string _APIKEY = "";
    
    public async Task GetApiKey()
    {
        var clientHandler = new HttpClientHandler();
        var client = new HttpClient(clientHandler);

        client.BaseAddress = new Uri($"https://mxtoolbox.com/api/v1/user"); // FCM HttpV1 API

        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = client.GetAsync($"https://mxtoolbox.com/api/v1/user")
            .Result;
        
        var jsonResponse = response.Content.ReadAsStringAsync().Result;
        var responseObj = JsonConvert.DeserializeObject<MXAuthUser>(jsonResponse);
        if (responseObj is not null)
            _APIKEY = responseObj.TempAuthKey;
    }

    public async Task<MXIPResponse?> CheckIp(string ip)
    {
        if (_APIKEY.Length == 0)
            return null;
        
        var clientHandler = new HttpClientHandler();
        var client = new HttpClient(clientHandler);
        var addr = $"https://mxtoolbox.com/api/v1/Lookup?command=blacklist&argument={ip}&format=1";
        client.BaseAddress = new Uri(addr);

        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("tempauthorization", _APIKEY);

        var response = client.GetAsync(addr)
            .Result;
        
        var jsonResponse = response.Content.ReadAsStringAsync().Result;
        var responseObj = JsonConvert.DeserializeObject<MXIPResponse>(jsonResponse);
        return responseObj;
    }
}