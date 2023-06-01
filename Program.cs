using System.Text;
using Newtonsoft.Json;

namespace SnappFoodChecker;

public class Program
{
    private static int _accNum;
    private static int MAX_RETRY_COUNT = 50;

    public static void Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("SnappFood Checker");
        Console.WriteLine("1 . check snappFood");
        Console.WriteLine("2 . Random code");
        Console.Write("Select: ");
        Console.ResetColor();
        var x = Convert.ToInt32(Console.ReadLine());
        if (x == 1)
        {
            try
            {
                var useridFilename = "userid.txt";
                var voucherFilename = "voucher.txt";
                var user_id_list = File.ReadAllText(useridFilename).Split("\n");
                var used = new List<string>();

                foreach (var line in File.ReadAllText(voucherFilename).Split("\n"))
                {
                    Thread.Sleep(2500);
                    var t = new Thread(async () =>
                        await SendReqAsync(line.Trim(), user_id_list[_accNum].Trim()));
                    t.Start();
                    used.Add(line.Trim());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return;
        }

        if (x == 2)
        {
            RandomVouchers.Program.Main2(args);
            Console.WriteLine("");
            Main(args);
        }

        Console.ReadKey();
    }

    private static async Task SendReqAsync(string token, string user_id, string uuid = null)
    {
        Thread.Sleep(500);
        var url =
            $"https://snappfood.ir/mobile/v2/basket/user-{user_id}?client=PSA&optionalClient=PSA&deviceType=PSA&appVersion=5.6.6&optionalVersion=5.6.6&UDID={uuid ?? Guid.NewGuid().ToString()}";
        var n4 = new { actions = new[] { new { action = "setVoucher", argument = new { voucher_code = token } } } };
        var head = new HttpClient().DefaultRequestHeaders;
        head.Add("accept", "application/json, text/plain, */*");
        head.Add("accept-encoding", "gzip, deflate, br");
        head.Add("accept-language", "en-US,en;q=0.9");
        head.Add("origin", "https://psa.snappfood.ir");
        head.Add("referer", "https://psa.snappfood.ir/");
        head.Add("sec-ch-ua", "\"Chromium\";v=\"107\", \"Google Chrome\";v=\"107\", \"Not;A=Brand\";v=\"24\"");
        head.Add("sec-ch-ua-mobile", "?0");
        head.Add("sec-ch-ua-platform", "Windows");
        head.Add("sec-fetch-dest", "empty");
        head.Add("sec-fetch-mode", "cors");
        head.Add("sec-fetch-site", "same-site");
        head.Add("user-agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");

        try
        {
            var res = await new HttpClient().PutAsync(url,
                new StringContent(JsonConvert.SerializeObject(n4), Encoding.UTF8));
            if (res.IsSuccessStatusCode)
            {
                var resContent = await res.Content.ReadAsStringAsync();
                if (resContent.Contains("https://www.google.com/recaptcha/api.js?hl="))
                {
                    Console.WriteLine("arvanRecaptcha");
                    return;
                }

                if (resContent.Length > 1)
                {
                    dynamic resJson = JsonConvert.DeserializeObject(resContent);

                    if (resJson.status == true)
                    {
                        foreach (var item in resJson.data.basket.prices)
                            if (item.alias == "VOUCHER_DISCOUNT_PRICE")
                            {
                                var value = item.value;
                                Console.WriteLine($"{token} is Alive => {value}T");
                                File.AppendAllText("Goods.txt", $"{token} => {value}\n");
                            }
                    }
                    else
                    {
                        Console.WriteLine($"{token} is Dead :| => {resJson.error.message} ");
                        if (!string.IsNullOrEmpty(resJson.message))
                            if (resJson.message.Contains("تعداد بروزرسانی"))
                            {
                                _accNum++;
                                Console.WriteLine("\n\nUser Changed :))\n\n");
                            }
                    }
                }
                else
                {
                    Console.WriteLine("The User is invalid");
                    _accNum++;
                    Console.WriteLine("\n\nUser Changed :))\n\n");
                }
            }
            else
            {
                Console.WriteLine($"Error: {res.StatusCode} - {res.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

}