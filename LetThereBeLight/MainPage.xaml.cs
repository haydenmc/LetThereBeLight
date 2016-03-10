using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Gpio;
using Windows.Devices.Pwm;
using PwmSoftware;
using System.Threading.Tasks;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LetThereBeLight
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
        }

        public async void Go(object sender, RoutedEventArgs e)
        {
            var pwmProvider = PwmProviderSoftware.GetPwmProvider();
            var pwmControllers = await PwmController.GetControllersAsync(pwmProvider);
            var pwmController = pwmControllers.FirstOrDefault();
            var hue = 0.0;
            pwmController.SetDesiredFrequency(200);
            var greenPin = pwmController.OpenPin(17);
            greenPin.SetActiveDutyCyclePercentage(0);
            greenPin.Start();
            var redPin = pwmController.OpenPin(27);
            redPin.SetActiveDutyCyclePercentage(0);
            redPin.Start();
            var bluePin = pwmController.OpenPin(22);
            bluePin.SetActiveDutyCyclePercentage(0);
            bluePin.Start();
            while (true)
            {
                hue += 0.5;
                if (hue >= 360)
                {
                    hue = 0;
                }
                var c = fromHSL(hue, 1, 0.5);
                greenPin.SetActiveDutyCyclePercentage(Convert.ToInt32(c.G) / 255.0);
                bluePin.SetActiveDutyCyclePercentage(Convert.ToInt32(c.B) / 255.0);
                redPin.SetActiveDutyCyclePercentage(Convert.ToInt32(c.R) / 255.0);
                await Task.Delay(10);
            }
        }

        public Color fromHSL(double h, double s, double l)
        {
            double c = (1 - Math.Abs(2 * l - 1)) * s;
            double hPrime = h / 60;
            double x = c * (1 - Math.Abs(hPrime % 2 - 1));

            double r1 = 0;
            double g1 = 0;
            double b1 = 0;
            if (hPrime >= 5)
            {
                r1 = c;
                g1 = 0;
                b1 = x;
            } else if (hPrime >= 4)
            {
                r1 = x;
                g1 = 0;
                b1 = c;
            } else if (hPrime >= 3)
            {
                r1 = 0;
                g1 = x;
                b1 = c;
            } else if (hPrime >= 2)
            {
                r1 = 0;
                g1 = c;
                b1 = x;
            } else if (hPrime >= 1)
            {
                r1 = x;
                g1 = c;
                b1 = 0;
            } else if (hPrime >= 0)
            {
                r1 = c;
                g1 = x;
                b1 = 0;
            }
            double m = l - 0.5 * c;
            double r = r1 + m;
            double g = g1 + m;
            double b = b1 + m;
            return Color.FromArgb(255, Convert.ToByte(r * 255), Convert.ToByte(g * 255), Convert.ToByte(b * 255));
        }
    }
}
