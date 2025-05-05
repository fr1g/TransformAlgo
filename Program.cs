// See https://aka.ms/new-console-template for more information

using TransformAlgo;

Console.WriteLine("Hello, World!");

double[,] A =
{
    {1, 7, 12},
    {8, -5, 0},
    {7, 8, 2},
};

var prompt = """
             Commands:
                - Encrypt (1)
                - Decrypt (2)
                - Exit (0)
                ---
             """;

while (true)
{
    Console.WriteLine(prompt);
    var input = Console.ReadLine()!.Trim();
    switch (input)
    {
        case "1" or "Encrypt":
            Console.WriteLine("Enter raw text:");
            var rawtext = Console.ReadLine();
            if(rawtext is null) break;
            var result = Algorithm.Encrypt(rawtext, A);
            Console.WriteLine($"=== RESULT ===\n{Algorithm.Stringify(result)}\n=== END RESULT ===");
            break;
        
        case "2" or "Decrypt":
            Console.WriteLine("Enter CYPHER text:");
            var cypher = Console.ReadLine();
            if(cypher is null) break;
            var got = Algorithm.Decrypt(Algorithm.ParseCypher(cypher), A);
            Console.WriteLine($"=== RESULT ===\n{got}\n=== END RESULT ===");
            break;
        
        case "0" or "Exit":
            Console.WriteLine("");
            return;
        
        default:
            Console.WriteLine("not matched.\n");
            break;
    }
}