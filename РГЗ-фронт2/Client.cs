using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace РГЗ_фронт
{
    class Client
    {
        static SerialPort _serialPort;
        static string cmd;

        public static void ClosePort()
        {
            _serialPort.Close();
        }

        public static void CreatePort()
        {
            _serialPort = new SerialPort();
            // Устанавливаем таймаут для чтения и записи
            _serialPort.ReadTimeout = 50000;
            _serialPort.WriteTimeout = 50000;
            _serialPort.Encoding = Encoding.UTF8;
        }

        public static void OpenPort()
        {
            // Открываем соединение последовательного порта
            _serialPort.Open();
            cmd = "";
        }

        // Отображдает существуюшие порты и задает выбранный порт
        public static void SetPortName(string newPortName)
        {
            _serialPort.PortName = "COM2";
            if (!(newPortName == "") && newPortName.ToLower().StartsWith("com"))
            {
                _serialPort.PortName = newPortName;
            }
        }
        // Задает скорость передачи для последовательного порта.
        public static void SetPortBaudRate(string newBaudRate)
        {
            _serialPort.BaudRate = 9600;
            if (newBaudRate != "")
            {
                _serialPort.BaudRate = int.Parse(newBaudRate);
            }
        }


        public static string[] FormMessage(string command, string args)
        {
            cmd = command;
            string[] value = new string[10];
            switch (command)
            {
                case "hello":
                    SendMessage(command, null);
                    value[0] = GetMessage(command, null);
                    break;
                case "auth":
                    SendMessage(command, args);
                    value[0] = GetMessage(command, null);
                    break;
                case "ls":
                    SendMessage(command, args);
                    value = GetLsMessage(command, null);
                    break;
                case "pwd":
                    SendMessage(command, null);
                    value[0] = GetMessage(command, null);
                    break;
                case "get":
                    SendMessage(command, args);
                    value[0] = GetMessage(command, null);
                    break;
                case "get2":
                    value = SendGetManyMessage(args);
                    break;
                case "cd":
                    SendMessage(command, args);
                    value[0] = GetMessage(command, null);
                    break;
                default:
                    MessageBox.Show("Несуществующая комманда", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
            return value;
        }

        public static void SendMessage(string command, string arg)
        {
            string[] args = null;
            if (arg != null)
                args = arg.Split(' ');

            string msg = "";
            byte[] checkSum;
            if (String.Equals(command, "hello") || String.Equals(command, "pwd") || String.Equals(command, "quit"))
                command = "hpq";
            switch (command)
            {
                case "hpq":
                    msg = cmd + ". ";
                    break;
                case "auth":
                    msg = command + " " + args[0] + " " + args[1] + ". ";
                    break;
                case "ls":
                    if (arg != null)
                        msg = command + " " + arg + ". ";
                    else
                        msg = command + ". ";
                    break;
                case "cd":
                    msg = command + " " + arg + ". ";
                    break;
                case "get":
                    msg = command + " " + arg + ". ";
                    break;
                default:
                    MessageBox.Show("Несуществующая комманда", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
            // Отправка сообщения
            byte[] message = Encoding.UTF8.GetBytes(msg);
            checkSum = CalcSimpleCheckSum(message);
            List<byte> messageListByte = message.ToList();
            for (int i = 0; i < checkSum.Length; i++)
            {
                messageListByte.Add(checkSum[i]);
            }
            byte[] MessageWithCheckSum = messageListByte.ToArray();
            _serialPort.WriteLine(Encoding.UTF8.GetString(MessageWithCheckSum));
        }

        public static byte[] CalcSimpleCheckSum(byte[] messageBytes)
        {
            byte[] checkSum;
            checkSum = new byte[4];
            byte cs = 0;

            foreach (byte b in messageBytes)
            {
                cs ^= b;
            }

            checkSum[0] = cs;
            return checkSum;
        }

        // Получение сообщений
        private static string GetMessage(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] receivedMessage = new byte[_serialPort.BytesToRead];
            string response = _serialPort.ReadLine();
            receivedMessage = Encoding.UTF8.GetBytes(response);

            string getMessage = Encoding.UTF8.GetString(receivedMessage);

            string[] values = getMessage.Split(' ');

            /*if (receivedMessage == null || values[0] != "200" || cmd != "auth")
            {
                Console.WriteLine("Пришло сообщение: \n" + Encoding.UTF8.GetString(receivedMessage));
                return "";
            }
            */
            // Проверка контрольной суммы
            if (CheckControlSum(receivedMessage))
                Console.WriteLine("Сообщение успешно доставлено. Контрольная сумма проверена");
            else
            {
                Console.WriteLine("Контрольная сумма не прошла проверку");
                return "";
            }

            switch (cmd)
            {
                case "hello":
                    switch (values[0])
                    {
                        case "200":
                            return values[1];
                    }
                    break;
                case "auth":
                    if (values[0] != "200")
                    {
                        throw new Exception(values[1]);
                    }
                    break; 
                case "cd":
                    switch (values[0])
                    {
                        case "200":
                            Console.WriteLine(values[1]);
                            break;
                        case "101":
                            Console.WriteLine(values[1]);
                            break;
                        case "102":
                            Console.WriteLine(values[1]);
                            break;
                        default:
                            Console.WriteLine(values[1]);
                            break;
                    }
                    break;
                case "get":
                { 
                    Console.WriteLine(values[1]);

                    string list = _serialPort.ReadLine();

                    Console.WriteLine("list = " + list);
                    string listfile = list.Remove(list.Length - 6);
                    listfile = list.Remove(0, 4);
                    string[] lf = listfile.Split('.');
                    listfile = lf[0];
                    Console.WriteLine(listfile);
                    return listfile;
                }
                case "pwd":
                    switch (values[0])
                    {
                        case "200":
                            Console.WriteLine(values[1]);
                            values[values.Length - 2] = values[values.Length - 2].TrimStart('.');
                            return values[values.Length - 2].TrimEnd('.');
                        default:
                            Console.WriteLine(values[1]);
                            break;
                    }
                    Console.WriteLine(String.Join(" ", values));
                    break;
                case "quit":
                    switch (values[0])
                    {
                        case "200":
                            Console.WriteLine(values[1]);
                            break;
                        default:
                            Console.WriteLine(values[1]);
                            break;
                    }
                    Console.WriteLine(String.Join(" ", values));
                    break;
                default:
                    MessageBox.Show("Несуществующая комманда", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }

            string code = values[0];
            Console.WriteLine(code);
            if (code == "200")
            {
                Console.WriteLine("Передача прошла успешно. Сообщение:\n");
                Console.WriteLine(values[1]);
            }
            return "";
        }

        private static string[] GetLsMessage(object sender, SerialDataReceivedEventArgs e)
        {
            // Чтение полученных байтов
            byte[] receivedMessage = new byte[_serialPort.BytesToRead];
            string response = _serialPort.ReadLine();
            receivedMessage = Encoding.UTF8.GetBytes(response);
            string getMessage = Encoding.UTF8.GetString(receivedMessage);

            string[] values = getMessage.Split(' ');

            if (receivedMessage == null || values[0] != "200")
            {
                Console.WriteLine("Пришло сообщение: \n" + Encoding.UTF8.GetString(receivedMessage));
                return null;
            }

            // Проверка контрольной суммы
            if (CheckControlSum(receivedMessage))
                Console.WriteLine("Сообщение успешно доставлено. Контрольная сумма проверена");
            else
            {
                Console.WriteLine("Контрольная сумма не прошла проверку");
                return null;
            }

            byte[] receivedMessage1 = new byte[Convert.ToInt32(values[values.Length - 3]) + 10];
            response = _serialPort.ReadLine();
            receivedMessage1 = Encoding.UTF8.GetBytes(response);

            string list = Encoding.UTF8.GetString(receivedMessage1);
            Console.WriteLine("list = " + list);
            if (list != "000 . \u001e\0\0\0")
            {
                string[] listvalues = list.Split(' ');
                List<string> lvl = new List<string>();
                for (int i = 1; i < listvalues.Length - 1; i++)
                {
                    if (listvalues[i].Contains("f-"))
                    {
                        listvalues[i] = listvalues[i].TrimStart('f');
                        listvalues[i] = listvalues[i].TrimStart('-');
                        lvl.Add(listvalues[i]);
                    }
                    else if (listvalues[i].Contains("d-"))
                    {
                        listvalues[i] = listvalues[i].TrimStart('d');
                        listvalues[i] = listvalues[i].TrimStart('-');
                        lvl.Add(listvalues[i]);
                    }
                }
                string[] listvalues2 = lvl.ToArray();
                listvalues2[listvalues2.Length - 1] = listvalues2[listvalues2.Length - 1].TrimEnd('.');
                return listvalues2;
            }

            return null;
        }

        public static string[] SendGetManyMessage(string args)
        {
            SendMessage("ls", args);
            string[] listvalues = GetLsMessage("ls", null);

            if (listvalues != null)
            {
                int len = listvalues.Length;

                string[] values = new string[len];

                FormMessage("cd", args);

                for (int i = 0; i < len; i++)
                {
                    values[i] = FormMessage("get", listvalues[i])[0];
                }

                FormMessage("cd", "..");

                return values;
            }

            return null;
            
        }

        private static bool CheckControlSum(byte[] messageBytes)
        {
            bool flagCheckSumEqual = false;
            // Последний байт в сообщении - контрольная сумма
            byte checkSumInMessage = messageBytes[messageBytes.Length - 4];
            // Вычисление сообщения без контрольной суммы
            byte[] array = new byte[messageBytes.Length];
            messageBytes.CopyTo(array, 0);
            Array.Resize(ref array, array.Length - 4);
            byte[] checkSum = CalcSimpleCheckSum(array);
            Console.WriteLine("Контрольная сумма посылки: " + $"0x{Encoding.UTF8.GetString(checkSum)}");
            // Сравнение двух сумм
            if (checkSum[0] == checkSumInMessage)
            {
                flagCheckSumEqual = true;
            }
            return flagCheckSumEqual;
        }


    }
}

