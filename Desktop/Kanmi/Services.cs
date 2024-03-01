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
    /// Quand la méthode <see cref="ICardReaderService.Start"/> est appelée,
    /// bloque le thread dans lequel est est appelée. La méthode <see cref="ICardReaderService.Stop"/>
    /// peut être appelée depuis un autre thread pour arrêter son exécution.
    /// </summary>
    /// <param name="serialDiscovery">Un objet permettant la découverte des ports série.</param>
    /// <returns>Une nouvelle instance de <see cref="ICardReaderService"/>.</returns>
    public static ICardReaderService MakeBlockingService(ISerialDiscovery serialDiscovery)
    {
        return new BlockingService(serialDiscovery);
    }

    [SupportedOSPlatform("Windows")]
    private static ISerialDiscovery DefaultSerialDiscovery()
    { 
        return new WindowsSerialDiscovery();
    }
}