using System.Runtime.Versioning;
using Kanmi.Serial;
using Kanmi.Serial.Windows;
using Kanmi.ServicesImplementation;

namespace Kanmi;

/// <summary>
/// Une fabrique qui fournit des instances de services.
/// </summary>
public static class Services
{
    /// <summary><inheritdoc cref="MakeBlockingService(ISerialDiscovery)" path="/summary"/></summary>
    /// <returns>Une nouvelle instance de <see cref="ICardReaderService"/>.</returns>
    /// <remarks>
    /// En utilisant cette surcharge, les dépendances sont fournies automatiquement, mais seulement Windows est
    /// supporté.
    /// </remarks>
    [SupportedOSPlatform("Windows")]
    public static ICardReaderService MakeBlockingService()
    {
        return MakeBlockingService(DefaultSerialDiscovery());
    }

    /// <summary>
    /// Crée un service qui s'exécute de manière synchrone et indéfiniment.
    /// Quand la méthode <see cref="ICardReaderService.Start"/> est appelée, elle
    /// bloque le thread dans lequel elle est appelée. La méthode <see cref="ICardReaderService.Stop"/>
    /// peut être appelée depuis un autre thread pour arrêter son exécution.
    /// </summary>
    /// <param name="serialDiscovery">Un objet permettant la découverte des ports série.</param>
    /// <returns>Une nouvelle instance de <see cref="ICardReaderService"/>.</returns>
    public static ICardReaderService MakeBlockingService(ISerialDiscovery serialDiscovery)
    {
        return new BlockingService(serialDiscovery);
    }
    
    /// <summary><inheritdoc cref="MakeBackgroundService(ISerialDiscovery)" path="/summary"/></summary>
    /// <returns>Une nouvelle instance de <see cref="ICardReaderService"/>.</returns>
    /// <remarks>
    /// En utilisant cette surcharge, les dépendances sont fournies automatiquement, mais seulement Windows est
    /// supporté.
    /// </remarks>
    [SupportedOSPlatform("Windows")]
    public static ICardReaderService MakeBackgroundService()
    {
        return MakeBackgroundService(DefaultSerialDiscovery());
    }

    /// <summary>
    /// Crée un service qui s'exécute sur un thread en arrière-plan.
    /// Quand la méthode <see cref="ICardReaderService.Start"/> est appelée, un thread est lancé et peut être arrêté
    /// en utilisant la méthode <see cref="ICardReaderService.Stop"/>. Attention, lorsque l'arrête est demandé, la
    /// méthode ne retournera pas tant que les opérations en cours soient terminées et le port fermé proprement, ce qui
    /// peut prendre jusqu'à 3 secondes.
    /// </summary>
    /// <param name="serialDiscovery">Un objet permettant la découverte des ports série.</param>
    /// <returns>Une nouvelle instance de <see cref="ICardReaderService"/>.</returns>
    public static ICardReaderService MakeBackgroundService(ISerialDiscovery serialDiscovery)
    {
        return new BackgroundService(serialDiscovery);
    }

    [SupportedOSPlatform("Windows")]
    private static ISerialDiscovery DefaultSerialDiscovery()
    { 
        return new WindowsSerialDiscovery();
    }
}