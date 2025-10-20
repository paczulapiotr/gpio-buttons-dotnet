using System.Device.Gpio;

Console.WriteLine("GPIO Test Program for Radxa Rock 5B. Press Ctrl+C to exit.");

// Używamy fizycznego schematu numeracji pinów (PinNumberingScheme.Board).
// Będziemy testować Pin 7 na 40-pinowym złączu.
const int TestPin = 7; 

// Pin 9 to popularny pin masy (GND) na 40-pinowym złączu.
Console.WriteLine($"Proszę połączyć Pin {TestPin} z masą (GND), np. z Pinem 9, aby wywołać zdarzenie.");

GpioController controller = null;
var cancellationTokenSource = new CancellationTokenSource();

Console.CancelKeyPress += (sender, eventArgs) => {
    eventArgs.Cancel = true;
    cancellationTokenSource.Cancel();
    Console.WriteLine("Zamykanie...");
};

try
{
    // Inicjalizujemy kontroler z fizycznym schematem numeracji.
    controller = new GpioController(PinNumberingScheme.Board);
    
    // Otwieramy pin jako wejście z rezystorem podciągającym (pull-up).
    // Oznacza to, że pin domyślnie jest w stanie WYSOKIM. Podłączenie go do GND pociągnie go do stanu NISKIEGO.
    controller.OpenPin(TestPin, PinMode.InputPullUp);
    Console.WriteLine($"Pomyślnie otwarto Pin {TestPin}. Obecna wartość: {controller.Read(TestPin)}");

    // Rejestrujemy callbacki dla OBU zboczy, opadającego i rosnącego, aby zobaczyć, czy cokolwiek się dzieje.
    controller.RegisterCallbackForPinValueChangedEvent(TestPin, PinEventTypes.Falling, OnPinEvent);
    controller.RegisterCallbackForPinValueChangedEvent(TestPin, PinEventTypes.Rising, OnPinEvent);

    Console.WriteLine("Nasłuchiwanie na zdarzenia...");

    // Czekamy na naciśnięcie Ctrl+C.
    cancellationTokenSource.Token.WaitHandle.WaitOne();
}
catch (Exception ex)
{
    Console.WriteLine($"Wystąpił błąd: {ex.Message}");
    Console.WriteLine("\nPorady dotyczące rozwiązywania problemów:");
    Console.WriteLine("1. Upewnij się, że uruchamiasz program z odpowiednimi uprawnieniami (np. jako root lub użytkownik w grupie 'gpio').");
    Console.WriteLine("2. Upewnij się, że pin nie jest używany przez inny proces lub moduł kernela.");
    Console.WriteLine("3. Sprawdź dokładnie swoje połączenia. Pin 7 powinien być połączony z pinem GND, np. Pinem 9.");
}
finally
{
    Console.WriteLine("Czyszczenie zasobów GPIO.");
    controller?.Dispose();
}

// Funkcja callback, która zostanie wykonana po zdarzeniu na pinie.
void OnPinEvent(object sender, PinValueChangedEventArgs args)
{
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] --- WYKRYTO ZDARZENIE! ---");
    Console.WriteLine($"Numer pinu: {args.PinNumber}");
    Console.WriteLine($"Typ zdarzenia: {args.ChangeType}");
}


