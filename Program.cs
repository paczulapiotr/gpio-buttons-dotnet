using System.Device.Gpio;

Console.WriteLine("Starting GPIO button listener. Press Ctrl+C to exit.");

// Define the GPIO pins for the buttons.
// Please change these to the actual pin numbers you are using.
const int TestPin = 14; // PIN10 - GPIO0_B6

var pins = new[] { TestPin };
var controller = new GpioController();
var cancellationTokenSource = new CancellationTokenSource();

// Register a handler for the Ctrl+C press
Console.CancelKeyPress += (sender, eventArgs) => {
    eventArgs.Cancel = true;
    cancellationTokenSource.Cancel();
    Console.WriteLine("Exiting...");
};

try
{
    foreach (var pin in pins)
    {
        // Open the pin for input with a pull-up resistor.
        // This means the pin will be HIGH by default, and LOW when the button is pressed (connected to ground).
        controller.OpenPin(pin, PinMode.InputPullUp);
        
        // Register a callback for the 'Falling' event (pin goes from HIGH to LOW)
        controller.RegisterCallbackForPinValueChangedEvent(pin, PinEventTypes.Falling, OnPinEvent);
    }

    Console.WriteLine("Listening for button presses...");

    // Wait until Ctrl+C is pressed
    cancellationTokenSource.Token.WaitHandle.WaitOne();
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}
finally
{
    Console.WriteLine("Cleaning up GPIO resources.");
    controller.Dispose();
}

// Callback function that will be executed on a pin event
void OnPinEvent(object sender, PinValueChangedEventArgs args)
{
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Button pressed on pin {args.PinNumber}. Event type: {args.ChangeType}");
}

