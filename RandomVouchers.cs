namespace RandomVouchers;

internal class RandomStringGenerator
{
    private readonly string prefix;
    private readonly Random random;

    public RandomStringGenerator(string prefix)
    {
        random = new Random();
        this.prefix = prefix;
    }

    public void GenerateRandomStrings(int count)
    {
        var length = 0;
        var validInput = false;
        while (!validInput)
        {
            Console.Write("Enter the length of the string: ");
            var input = Console.ReadLine();

            if (int.TryParse(input, out length))
                validInput = true;
            else
                Console.WriteLine("Invalid input. Please enter a valid number.");
        }

        for (var i = 0; i < count; i++)
        {
            var randomString = GenerateRandomString(length);
            var result = prefix + randomString; 

            Console.WriteLine(result);
            File.AppendAllText("voucher.txt", result + "\n");
        }

        Console.WriteLine("Saved successfully voucher.txt");
    }

    private string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var result = new char[length];

        for (var i = 0; i < length; i++) result[i] = chars[random.Next(chars.Length)];

        return new string(result); 
    }
}

public class Program
{
    public static void Main2(string[] args)
    {
        try
        {
            if (File.Exists("voucher.txt"))
            {
                Console.Write(
                    "The file voucher.txt already exists. Do you want to delete it?\nEnter 1 for Delete, 2 for Skip:");
                var input = Console.ReadLine() ?? "JUST GIVE ME NUMBER!!!";

                if (input == "1")
                {
                    File.Delete("voucher.txt");
                    Console.WriteLine("The file voucher.txt has been deleted.");
                }
                else
                {
                    Console.WriteLine("The file voucher.txt has not been deleted.");
                }
            }
            else
            {
                Console.WriteLine("The file voucher.txt does not exist.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        Console.Write("Give me the algorithm Example'NFAJ': ");
        var prefix = Console.ReadLine() ?? "JUST GIVE ME WORD!!!";
        var generator = new RandomStringGenerator(prefix);
        var length = 0;
        var validInput = false;
        while (!validInput)
        {
            Console.Write("Count: ");
            var input = Console.ReadLine();

            if (int.TryParse(input, out length))
                validInput = true;
            else
                Console.WriteLine("Invalid input. Please enter a valid number.");
        }

        generator.GenerateRandomStrings(length);
    }
}