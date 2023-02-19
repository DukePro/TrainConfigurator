namespace TrainConfigurator
{
    class Programm
    {
        static void Main()
        {
            Menu menu = new Menu();
            menu.ShowMenu();
        }
    }

    class Menu
    {
        private Direction _direction = new Direction();
        private Configurator _config = new Configurator();

        public void ShowMenu()
        {
            const string MenuCreateDirection = "1";
            const string MenuSellTickets = "2";
            const string MenuCompileTrain = "3";
            const string MenuShowTrainParam = "4";
            const string MenuSendTrain = "5";
            const string MenuExit = "0";

            bool isExit = false;

            string userInput;

            while (isExit == false)
            {
                ShowStatus();

                Console.WriteLine("\nМеню:");
                Console.WriteLine(MenuCreateDirection + " - Создать направление");
                Console.WriteLine(MenuSellTickets + " - Продать билеты");
                Console.WriteLine(MenuCompileTrain + " - Сформировать состав");
                Console.WriteLine(MenuShowTrainParam + " - Показать параметры состава");
                Console.WriteLine(MenuSendTrain + " - Отправить поезд");
                Console.WriteLine(MenuExit + " - Выход");

                CleanConsoleString();
                userInput = Console.ReadLine();
                CleanConsoleBelowLine();

                switch (userInput)
                {
                    case MenuCreateDirection:
                        _direction.CreateDirection(_config.Passengers);
                        break;

                    case MenuSellTickets:
                        _config.SellTickets(_direction.IsDirectionSet);
                        break;

                    case MenuCompileTrain:
                        _config.CompileTrain();
                        break;

                    case MenuShowTrainParam:
                        _config.ShowTrainParameters();
                        break;

                    case MenuSendTrain:
                        SendTrain();
                        break;

                    case MenuExit:
                        isExit = true;
                        break;
                }
            }
        }

        private void ShowStatus()
        {
            int infoPositionY = 0;
            string readyToGo;

            if (_direction.IsDirectionSet && _config.ReadyStatus())
            {
                readyToGo = "ГОТОВ";
            }
            else
            {
                readyToGo = "НЕ ГОТОВ";
            }

            Console.SetCursorPosition(0, infoPositionY);
            CleanConsoleString();
            Console.WriteLine($"Направление: {_direction.GetDirection()} | Билетов продано: {_config.GetTickets()} | Статус поезда: {_config.GetTrainStatus()} | К отправке: {readyToGo}");
        }

        private void CleanConsoleBelowLine()
        {
            int currentLineCursor = Console.CursorTop;

            for (int i = currentLineCursor; i < Console.WindowHeight; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(0, currentLineCursor);
        }

        private void CleanConsoleString()
        {
            Console.CursorLeft = 0;

            int currentLineCursor = Console.CursorTop;

            Console.Write(new string(' ', Console.WindowWidth));
            Console.CursorTop = currentLineCursor;
            Console.SetCursorPosition(0, currentLineCursor);
        }

        private void SendTrain()
        {
            if (_direction.IsDirectionSet && _config.ReadyStatus())
            {
                _config.ClearTrainAndPax();
                _direction.ClearDirection();
                Console.WriteLine("Поезд отправлен.");
            }
            else
            {
                Console.WriteLine("Поезд не готов к отправке.");
            }
        }
    }

    class Direction
    {
        public bool IsDirectionSet { get; private set; }

        private string _directionFrom;
        private string _directionTo;

        public void CreateDirection(int passagers)
        {
            if (passagers == 0)
            {
                Console.WriteLine("Ведите пункт отправления");
                _directionFrom = Console.ReadLine();

                Console.WriteLine("Ведите пункт прибытия");
                _directionTo = Console.ReadLine();

                Console.WriteLine($"Направление \"{_directionFrom} - {_directionTo}\" создано.");
            }
            else
            {
                Console.WriteLine("Нельзя изменить направление, на которое уже проданы билеты.");
            }
        }

        public string GetDirection()
        {
            string direction;

            if (_directionFrom != null && _directionTo != null)
            {
                IsDirectionSet = true;
                return direction = ($"{_directionFrom} - {_directionTo}");
            }
            else
            {
                IsDirectionSet = false;
                return direction = ("Не установлено");
            }
        }

        public void ClearDirection()
        {
            _directionFrom = null;
            _directionTo = null;
        }
    }

    class Configurator
    {
        public int Passengers { get; private set; }

        private List<Wagon> _trainComposition = new List<Wagon>();

        private Random _rand = new Random();
        private WagonLux _lux = new WagonLux();
        private WagonSV _sv = new WagonSV();
        private WagonCoupe _coupe = new WagonCoupe();
        private WagonReservedSeat _reservedSeat = new WagonReservedSeat();

        public void SellTickets(bool isDirectionExist)
        {
            if (isDirectionExist)
            {
                if (Passengers == 0)
                {
                    Passengers = _rand.Next(200, 648);

                    Console.WriteLine("Продано билетов - " + Passengers);
                }
                else
                {
                    Console.WriteLine("Билеты уже проданы");
                }
            }
            else
            {
                Console.WriteLine("Сначала нужно создать направление");
            }
        }

        public int GetTickets()
        {
            return Passengers;
        }

        public void ShowTrainParameters()
        {
            if (ReadyStatus())
            {
                foreach (var wagon in _trainComposition)
                {
                    Console.WriteLine($"Количество вагонов - {wagon.Units}, Тип вагона - {wagon.Name}");
                }
            }
            else
            {
                Console.WriteLine("Состав ещё не сформирован.");
            }
        }

        public string GetTrainStatus()
        {
            if (_trainComposition.Count > 0)
            {
                return "Сформирован";
            }
            else
            {
                return "Не сформирован";
            }
        }

        public void CompileTrain()
        {
            int passengers = Passengers;
            bool isAllHaveSeat = false;

            _trainComposition.Clear();

            if (passengers > 0)
            {
                while (isAllHaveSeat == false)
                {
                    if (passengers >= _reservedSeat.Seats)
                    {
                        _reservedSeat.Units = passengers / _reservedSeat.Seats;
                        _trainComposition.Add(_reservedSeat);
                        passengers -= _reservedSeat.Units * _reservedSeat.Seats;

                        if (passengers <= 0)
                        {
                            isAllHaveSeat = true;
                        }
                    }
                    else if (passengers >= _coupe.Seats)
                    {
                        _coupe.Units = passengers / _coupe.Seats;
                        _trainComposition.Add(_coupe);
                        passengers -= _coupe.Units * _coupe.Seats;

                        if (passengers <= 0)
                        {
                            isAllHaveSeat = true;
                        }
                    }
                    else if (passengers >= _sv.Seats)
                    {
                        _sv.Units = passengers / _sv.Seats;
                        _trainComposition.Add(_sv);
                        passengers -= _sv.Units * _sv.Seats;

                        if (passengers <= 0)
                        {
                            isAllHaveSeat = true;
                        }
                    }
                    else if (passengers > 0)
                    {
                        double tempPax = passengers;

                        tempPax = tempPax / _lux.Seats;
                        _lux.Units = Convert.ToInt32(Math.Ceiling(tempPax));
                        _trainComposition.Add(_lux);
                        passengers -= _lux.Units * _lux.Seats;

                        if (passengers <= 0)
                        {
                            isAllHaveSeat = true;
                        }
                    }
                }

                Console.WriteLine("Состав сформирован");
            }
            else
            {
                Console.WriteLine("Билеты на рейс ещё не проданы");
            }
        }

        public bool ReadyStatus()
        {
            if (Passengers > 0 && _trainComposition.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ClearTrainAndPax()
        {
            _trainComposition.Clear();
            Passengers = 0;
        }
    }

    class Wagon
    {
        public Wagon(int units, int seats, string name)
        {
            Units = units;
            Seats = seats;
            Name = name;
        }

        public int Units { get; set; }
        public int Seats { get; protected set; }
        public string Name { get; protected set; }

        public Wagon()
        {
        }
    }

    class WagonLux : Wagon
    {
        public WagonLux()
        {
            Units = 0;
            Seats = 6;
            Name = "Люкс";
        }
    }

    class WagonSV : Wagon
    {
        public WagonSV()
        {
            Units = 0;
            Seats = 18;
            Name = "СВ";
        }
    }

    class WagonCoupe : Wagon
    {
        public WagonCoupe()
        {
            Units = 0;
            Seats = 36;
            Name = "Купе";
        }
    }

    class WagonReservedSeat : Wagon
    {
        public WagonReservedSeat()
        {
            Units = 0;
            Seats = 54;
            Name = "Плацкарт";
        }
    }
}