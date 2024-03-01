using Kanmi;

namespace KanmiDebug;

public class LogListener: ICardReaderListener
{
    private static void Log(string infos)
    {
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {infos}");
    }
    
    public void OnReaderConnected(string readerInfos)
    {
        Log($"Nouvel appareil connecté : {readerInfos}");
    }

    public void OnReaderDisconnected()
    {
        Log($"L'appareil a été déconnecté");
    }

    public void OnEngagedWithPicc(PiccUid uid)
    {
        Log($"Carte lue : {uid.AsString}");
    }

    public void OnUnexpectedError(ErrorContext error)
    {
        Log("Exception non gérée =======================================");
        Console.WriteLine(error.StackTrace);
        Console.WriteLine($"{error.ErrorType}: {error.ErrorMessage}");
        Console.WriteLine("=================================================================================");
        
        error.WasHandled = true;
    }
}