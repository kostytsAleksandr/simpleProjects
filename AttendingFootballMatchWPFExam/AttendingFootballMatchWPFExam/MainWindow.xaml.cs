using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LoggerLib;

namespace AttendingFootballMatchWPFExam
{

    public partial class MainWindow : Window
    {
        private MyTxtLogger _logger;
        private BlockingCollection<Fan> _bagInFrontOfTheStadium = new BlockingCollection<Fan>(
        new ConcurrentBag<Fan>());
        private BlockingCollection<Fan> _queueInFrontOfTheSectors = new BlockingCollection<Fan>(
            new ConcurrentQueue<Fan>());
        private BlockingCollection<Fan> _bagFillingTheSector1 = new BlockingCollection<Fan>(
      new ConcurrentBag<Fan>());
        private BlockingCollection<Fan> _bagFillingTheSector2 = new BlockingCollection<Fan>(
      new ConcurrentBag<Fan>());
        private BlockingCollection<Fan> _bagFillingTheSector3 = new BlockingCollection<Fan>(
      new ConcurrentBag<Fan>());
        private BlockingCollection<Fan> _bagFillingTheSector4 = new BlockingCollection<Fan>(
      new ConcurrentBag<Fan>());
        private BlockingCollection<Fan> _bagFillingTheSector5 = new BlockingCollection<Fan>(
      new ConcurrentBag<Fan>());
        private BlockingCollection<Fan> _bagFillingTheSector6 = new BlockingCollection<Fan>(
      new ConcurrentBag<Fan>());
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private static SynchronizationContext _context = SynchronizationContext.Current;

        private void GenerateQueueInFrontOfSectors(int firstPlace, int lastPlace)
        {
            _logger.WriteProtocol("started", $"queue in front of sectors,{firstPlace}-{lastPlace}", $"{Task.CurrentId}");
            for (int sector = 1; sector <= 6 && _bagInFrontOfTheStadium.IsCompleted != true; sector++)
            {
                for (int place = firstPlace; place <= lastPlace &&
                    _bagInFrontOfTheStadium.IsCompleted != true;)
                {
                        Fan tmpFan = _bagInFrontOfTheStadium.Take(_cts.Token);
                        if (tmpFan.Hasticket == true)
                        {
                            tmpFan.NumberOfPlace = place;
                            tmpFan.NumberOfSector = sector;
                            _queueInFrontOfTheSectors.Add(tmpFan);
                            Thread.Sleep(1);
                            _context.Send(//Метод работает синхронно, текущий поток дождется его завершения
                                         //Post будет работать асинхронно, текущий поток его не ждет
                                state =>
                                {
                                    tBlockQueueBeforeSectors.Text = (_queueInFrontOfTheSectors.Count.ToString());
                                    tBlockQueueBeforeStadium.Text = _bagInFrontOfTheStadium.Count.ToString();
                                },
                                null
                                );
                            place++;
                        }                 
                    }
            }
            _logger.WriteProtocol("finished", $"queue in front of sectors,{firstPlace}-{lastPlace}", $"{Task.CurrentId}");
        }
        private void GenerateQueueInFrontOfStadium(int capacityStadium)
        {
            _logger.WriteProtocol("started", $"queue in front of stadion", $"{Task.CurrentId}");
            int hasTicket = 0;
            for (int i = 0; i < capacityStadium * 2 && hasTicket < capacityStadium; i++)
            {
                bool flagTicket = false;
                if (i % 2 == 0)
                {
                    flagTicket = true;
                    hasTicket++;
                }
                Fan fan = new Fan(flagTicket);
                _bagInFrontOfTheStadium.Add(fan);
                _context.Send(//Метод работает синхронно, текущий поток дождется его завершения
                             //Post будет работать асинхронно, текущий поток его не ждет
                    state => { tBlockQueueBeforeStadium.Text = _bagInFrontOfTheStadium.Count.ToString(); },
                    null
                    );
            }
            _logger.WriteProtocol("finished", $"queue in front of stadion", $"{Task.CurrentId}");
        }
        private void FillingSectors(int numberOfSector)
        {
            _logger.WriteProtocol("started", $"filling sectors,{numberOfSector}", $"{Task.CurrentId}");
            TextBlock ref_textBlock = null;
            ListBox ref_listbox = null;
            BlockingCollection<Fan> ref_BagFillingTheSector = null;
            switch (numberOfSector)
            {
                case 1:
                    {
                        ref_BagFillingTheSector = _bagFillingTheSector1;
                        ref_textBlock = tBlockBusyPlacesSecotr1;
                        ref_listbox = lBoxSector1;
                        break;
                    }
                case 2:
                    {
                        ref_BagFillingTheSector = _bagFillingTheSector2;
                        ref_textBlock = tBlockBusyPlacesSecotr2;
                        ref_listbox = lBoxSector2;
                        break;
                    }
                case 3:
                    {
                        ref_BagFillingTheSector = _bagFillingTheSector3;
                        ref_textBlock = tBlockBusyPlacesSecotr3;
                        ref_listbox = lBoxSector3;
                        break;
                    }
                case 4:
                    {
                        ref_BagFillingTheSector = _bagFillingTheSector4;
                        ref_textBlock = tBlockBusyPlacesSecotr4;
                        ref_listbox = lBoxSector4;
                        break;
                    }
                case 5:
                    {
                        ref_BagFillingTheSector = _bagFillingTheSector5;
                        ref_textBlock = tBlockBusyPlacesSecotr5;
                        ref_listbox = lBoxSector5;
                        break;
                    }
                case 6:
                    {
                        ref_BagFillingTheSector = _bagFillingTheSector6;
                        ref_textBlock = tBlockBusyPlacesSecotr6;
                        ref_listbox = lBoxSector6;
                        break;
                    }
            }
            while (_queueInFrontOfTheSectors.IsCompleted != true &&
                ref_BagFillingTheSector.Count != 10000)
            {
                Thread.Sleep(1);
                Fan tmpFan = null;
                    _queueInFrontOfTheSectors.TryTake(out tmpFan, 100, _cts.Token);
                    if (tmpFan != null && tmpFan.NumberOfSector == numberOfSector)
                    {
                        ref_BagFillingTheSector.Add(tmpFan);

                        _context.Send(//Метод работает синхронно, текущий поток дождется его завершения
                                     //Post будет работать асинхронно, текущий поток его не ждет
                                 state =>
                                 {
                                     ref_textBlock.Text = ref_BagFillingTheSector.Count.ToString();
                                     ref_listbox.Items.Add(tmpFan.NumberOfPlace.ToString());
                                     tBlockQueueBeforeSectors.Text = (_queueInFrontOfTheSectors.Count.ToString());
                                     tBlockFansOnTheirPlaces.Text = (_bagFillingTheSector1.Count +
                                     _bagFillingTheSector2.Count +
                                     _bagFillingTheSector3.Count +
                                     _bagFillingTheSector4.Count +
                                     _bagFillingTheSector5.Count +
                                     _bagFillingTheSector6.Count).ToString();
                                 },
                                 null
                                 );
                    }
                    else if (tmpFan != null)
                    {
                        _queueInFrontOfTheSectors.Add(tmpFan);
                    }
            }
            _logger.WriteProtocol("finished", $"filling sectors,{numberOfSector}", $"{Task.CurrentId}");
        }
        public MainWindow()
        {
            InitializeComponent();
            _logger = MyTxtLogger.CreateTxtLogger(false);
            _logger.Use_E = true;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                _logger.WriteProtocol("started", "btnStartTask", $"{Task.CurrentId}");
                Task task = Task.Run(() =>
                GenerateQueueInFrontOfStadium(60000));
          
                Task[] tasks = new Task[4];
                tasks[0] = Task.Factory.StartNew(() =>
                  GenerateQueueInFrontOfSectors(1, 2500));
                tasks[1] = Task.Factory.StartNew(() =>
                  GenerateQueueInFrontOfSectors(2501, 5000));
                tasks[2] = Task.Factory.StartNew(() =>
              GenerateQueueInFrontOfSectors(5001, 7500));
                tasks[3] = Task.Factory.StartNew(() =>
              GenerateQueueInFrontOfSectors(7501, 10000));
            
                Task[] tasks1 = new Task[6];
                tasks1[0] = Task.Factory.StartNew(() => FillingSectors(1));
    
                tasks1[1] = Task.Factory.StartNew(() => FillingSectors(2));

                tasks1[2] = Task.Factory.StartNew(() => FillingSectors(3));

                tasks1[3] = Task.Factory.StartNew(() => FillingSectors(4));

                tasks1[4] = Task.Factory.StartNew(() => FillingSectors(5));

                tasks1[5] = Task.Factory.StartNew(() => FillingSectors(6));

                Task.WaitAll(tasks1);
                _logger.WriteProtocol("ended", "btnEndTask", $"{Task.CurrentId}");
            });
        }

        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            _cts.Cancel();
        }
    }
}
