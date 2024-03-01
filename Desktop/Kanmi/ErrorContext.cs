namespace Kanmi;

/// <summary>
/// Encapsule une erreur à gérer.
/// </summary>
public class ErrorContext
{
    private readonly Exception exception;
    private bool wasHandled;

    /// <summary>
    /// Crée un nouveau contexte à partir d'une exception.
    /// </summary>
    /// <param name="exception">L'exception depuis laquelle extraire les informations.</param>
    public ErrorContext(Exception exception)
    {
        this.exception = exception;
        this.wasHandled = false;
    }

    /// <summary>
    /// La pile des appels qui ont mené à cette erreur.
    /// </summary>
    public string StackTrace => this.exception.StackTrace ?? "";
    
    /// <summary>
    /// Le type de l'erreur.
    /// </summary>
    public string ErrorType => this.exception.GetType().Name;
    
    /// <summary>
    /// Le message d'erreur.
    /// </summary>
    public string ErrorMessage => this.exception.Message;

    /// <summary>
    /// Indique si cette erreur a été gérée ou non.
    /// </summary>
    public bool WasHandled
    {
        get => this.wasHandled;
        set => this.wasHandled = value;
    }
}