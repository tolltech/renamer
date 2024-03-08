// See https://aka.ms/new-console-template for more information

var rootFolderPath = args[0];
Console.WriteLine($"Will process {rootFolderPath}");

var writeMode = args.Length > 1 && args[1].ToLower() == "-w"; 

var files = GetAllFileRecursively(rootFolderPath).ToArray();
Console.WriteLine($"Found {files.Length} files");

for (var i = 0; i < files.Length; i++)
{
    var fileName = files[i];
    Console.WriteLine($"Processing {i+1} file. {fileName}");

    var file = new FileInfo(fileName);
    var newName = GetNewName(file.Name);

    if (string.IsNullOrWhiteSpace(newName))
    {
        Console.WriteLine($"Processing {i+1} file. New name for {file.Name} is empty, skipping");
        continue;
    }

    var newFullName = Path.Combine(file.DirectoryName!, newName);

    if (File.Exists(newFullName))
    {
        Console.WriteLine($"Processing {i+1} file. {newFullName} already exists, skipping");
        continue;
    }

    Console.WriteLine($"Processing {i+1} file. {file.FullName} -> {newFullName}. Saving");
    if (writeMode)
    {
        File.Move(file.FullName, newFullName);
    }
}

static string? GetNewName(string oldName)
{
    try
    {
        //Gintama (3).mkv.cover.jpg

        if (!oldName.Contains("(") || !oldName.Contains(")"))
        {
            Console.WriteLine($"OldFileName contains invalid number of braces");
            return null;
        }

        var numberWithBraces =
            new string(
                oldName.SkipWhile(c => c != '(').TakeWhile(c => c != ')').ToArray()
            ) + ")";

        if (oldName.IndexOf(numberWithBraces, StringComparison.InvariantCulture) != oldName.LastIndexOf(numberWithBraces, StringComparison.InvariantCulture))
        {
            Console.WriteLine($"OldFileName contains more tha one int number");
            return null;
        }

        var number = int.Parse(new string(numberWithBraces.Where(char.IsDigit).ToArray()));
        return oldName.Replace(numberWithBraces, $".s01e{number:D3}.").Replace("..", ".");
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return null;
    }
}

static IEnumerable<string> GetAllFileRecursively(string sDir)
{
    foreach (var f in Directory.GetFiles(sDir))
    {
        yield return f;
    }
    
    foreach (var d in Directory.GetDirectories(sDir))
    {
        foreach (var f in GetAllFileRecursively(d))
        {
            yield return f;
        }
    }
}