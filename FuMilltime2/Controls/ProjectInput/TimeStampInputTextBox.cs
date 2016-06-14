namespace FuMilltime2.Controls.ProjectInput
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;

    [ToolboxItem(false)]
    public class TimeStampInputTextBox : TextBox
    {
        public TimeStampInputTextBox()
        {
            this.CausesValidation = true;
            this.TextChanged += this.HandleChangedValue;
            this.Width = 60;
            this.KeyPress += this.ReplaceNWithCurrentTime;
        }

        private void ReplaceNWithCurrentTime(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'n' || e.KeyChar == 'N')
            {
                e.Handled = true;
                this.Paste(DateTime.Now.ToString("HHmm"));
            }
        }

        public event MinutesChangedEventHandler MinutesChanged;

        public int TotalMinutes { get; private set; }

        private static int ParseMinutes(string text)
        {
            TimeSpan ts;
            if (TimeFieldParser.TryParse(text, out ts))
            {
                return (int)ts.TotalMinutes;
            }

            return -1;
        }

        private void HandleChangedValue(object sender, EventArgs e)
        {
            int newValue;

            if (string.IsNullOrWhiteSpace(this.Text))
            {
                newValue = 0;
                this.BackColor = Color.White;
            }
            else
            {
                // Get the input value, and convert any commas to periods
                var text = this.Text.Replace(',', '.');

                var minutes = ParseMinutes(text);
                if (minutes > 0)
                {
                    newValue = minutes;
                    this.BackColor = Color.LightGreen;
                }
                else
                {
                    newValue = 0;
                    this.BackColor = Color.LightPink;
                }
            }

            if (this.TotalMinutes != newValue)
            {
                this.TotalMinutes = newValue;
                this.OnMinutesChanged();
            }
        }

        private void OnMinutesChanged()
        {
            if (this.MinutesChanged != null)
            {
                this.MinutesChanged(this, EventArgs.Empty);
            }
        }

        private static class TimeFieldParser
        {
            public static bool TryParse(string s, out TimeSpan ts)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    if (s.Contains("-") && TryParseInterval(s, out ts))
                    {
                        return true;
                    }

                    if (TryParseDuration(s, out ts))
                    {
                        return true;
                    }
                }

                ts = new TimeSpan();
                return false;
            }

            private static bool ParseDateTime(string s, out DateTime d)
            {
                bool flag;
                Exception exception;
                d = DateTime.MinValue;
                switch (s.Length)
                {
                    case 1:
                        {
                            s = string.Format("0{0}00", s);
                            return DateTime.TryParseExact(
                                s, 
                                "HHmm", 
                                CultureInfo.InvariantCulture, 
                                DateTimeStyles.None, 
                                out d);

                            // try
                            // {
                            // d = DateTime.ParseExact(s, "HHmm", new CultureInfo(CultureInfo.CurrentCulture.Name));
                            // flag = true;
                            // }
                            // catch (Exception exception1)
                            // {
                            // exception = exception1;
                            // flag = false;
                            // }

                            // return flag;
                        }

                    case 2:
                        {
                            s = string.Format("{0}00", s);
                            return DateTime.TryParseExact(
                                s, 
                                "HHmm", 
                                CultureInfo.InvariantCulture, 
                                DateTimeStyles.None, 
                                out d);

                            // try
                            // {
                            // d = DateTime.ParseExact(s, "HHmm", new CultureInfo(CultureInfo.CurrentCulture.Name));
                            // flag = true;
                            // }
                            // catch (Exception exception1)
                            // {
                            // exception = exception1;
                            // flag = false;
                            // }

                            // return flag;
                        }

                    case 3:
                        {
                            s = string.Format("0{0}", s);
                            return DateTime.TryParseExact(
                                s, 
                                "HHmm", 
                                CultureInfo.InvariantCulture, 
                                DateTimeStyles.None, 
                                out d);

                            // try
                            // {
                            // d = DateTime.ParseExact(s, "HHmm", new CultureInfo(CultureInfo.CurrentCulture.Name));
                            // flag = true;
                            // }
                            // catch (Exception exception1)
                            // {
                            // exception = exception1;
                            // flag = false;
                            // }

                            // return flag;
                        }

                    case 4:
                        {
                            return DateTime.TryParseExact(
                                s, 
                                "HHmm", 
                                CultureInfo.InvariantCulture, 
                                DateTimeStyles.None, 
                                out d);

                            // try
                            // {
                            // d = DateTime.ParseExact(s, "HHmm", new CultureInfo(CultureInfo.CurrentCulture.Name));
                            // flag = true;
                            // }
                            // catch (Exception exception1)
                            // {
                            // exception = exception1;
                            // flag = false;
                            // }

                            // return flag;
                        }
                }

                return false;
            }

            private static bool TryParseDuration(string s, out TimeSpan ts)
            {
                int num;
                int num1;
                double num2;
                ts = new TimeSpan();
                if (string.IsNullOrWhiteSpace(s))
                {
                    return false;
                }

                if (s.Contains(":"))
                {
                    string[] strArrays = s.Split(new[] { ':' });
                    if (!int.TryParse(strArrays[0], out num) || !int.TryParse(strArrays[1], out num1) || num < 0
                        || num1 < 0)
                    {
                        return false;
                    }

                    ts = new TimeSpan(num, num1, 0);
                    return true;
                }

                if (!s.Contains(".") && !s.Contains(","))
                {
                    if (!int.TryParse(s, out num1) || num1 < 0)
                    {
                        return false;
                    }

                    ts = new TimeSpan(0, num1, 0);
                    return true;
                }

                s = s.Replace('.', ',');
                if (!double.TryParse(s, out num2) || num2 < 0)
                {
                    return false;
                }

                ts = new TimeSpan(0, (int)Math.Round(num2 * 60), 0);
                return true;
            }

            private static bool TryParseInterval(string s, out TimeSpan ts)
            {
                DateTime dateTime;
                DateTime dateTime1;
                string[] strArrays = s.Split(new[] { '-' });
                if (!ParseDateTime(strArrays[0], out dateTime) || !ParseDateTime(strArrays[1], out dateTime1)
                    || !(dateTime1 > dateTime))
                {
                    ts = new TimeSpan();
                    return false;
                }

                ts = dateTime1 - dateTime;
                return true;
            }
        }
    }

    public delegate void MinutesChangedEventHandler(object sender, EventArgs args);
}