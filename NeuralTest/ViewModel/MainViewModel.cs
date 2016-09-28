using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using AForge.Neuro;
using AForge.Neuro.Learning;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuralTest.Model;

namespace NeuralTest.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        private string _learningRate;
        private string _momentum;
        private string _sigmoidAlpha;
        private string _numberOfRecords;
        private string _hiddenNeuronsNumber;
        private string _iterationsNumber;

        private string _errorNumber;
        private bool _isLearningDone;
        private bool _isNotWorking;

        private string _year;
        private string _month;
        private string _day;
        private string _hour;
        private string _minute;
        private string _width;
        private string _height;

        private string _oneTryCrimeNumber;
        private IEnumerable<TrainingModel> _dataList;
       
        public ICommand LoadCmd { get; set; }
        public ICommand DeleteNetworkCmd { get; set; }
        public ICommand UseNetworkCmd { get; set; }
        public ICommand LoadTrainingDataCmd { get; set; }
        public ICommand LoadTestDataCmd { get; set; }
        public ICommand StartLearningCmd { get; set; }
        public ICommand OneTryCmd { get; set; }
        

        private Thread _workerThread = null;
        public MainViewModel()
        {
            InitializeCommands();
        }
        public string LearningRate
        {
            get { return _learningRate; }
            set { Set(ref _learningRate, value); }
        }
        public string Momentum
        {
            get { return _momentum; }
            set { Set(ref _momentum, value); }
        }
        public string SigmoidAlpha
        {
            get { return _sigmoidAlpha; }
            set { Set(ref _sigmoidAlpha, value); }
        }
        public string NumberOfRecords
        {
            get { return _numberOfRecords; }
            set { Set(ref _numberOfRecords, value); }
        }
        public string HiddenNeuronsNumber
        {
            get { return _hiddenNeuronsNumber; }
            set { Set(ref _hiddenNeuronsNumber, value); }
        }
        public string IterationsNumber
        {
            get { return _iterationsNumber; }
            set { Set(ref _iterationsNumber, value); }
        }
        public string ErrorNumber
        {
            get { return _errorNumber; }
            set { Set(ref _errorNumber, value); }
        }
        public bool IsLearningDone
        {
            get { return _isLearningDone; }
            set { Set(ref _isLearningDone, value); }
        }
        public bool IsNotWorking
        {
            get { return _isNotWorking; }
            set { Set(ref _isNotWorking, value); }
        }
        
        public string Year
        {
            get { return _year; }
            set { Set(ref _year, value); }
        }
        public string Month
        {
            get { return _month; }
            set { Set(ref _month, value); }
        }
        public string Day
        {
            get { return _day; }
            set { Set(ref _day, value); }
        }
        public string Hour
        {
            get { return _hour; }
            set { Set(ref _hour, value); }
        }
        public string Minute
        {
            get { return _minute; }
            set { Set(ref _minute, value); }
        }
        public string Width
        {
            get { return _width; }
            set { Set(ref _width, value); }
        }
        public string Height
        {
            get { return _height; }
            set { Set(ref _height, value); }
        }

        public IEnumerable<TrainingModel> DataList
        {
            get { return _dataList; }
            set { Set(ref _dataList, value); }
        }
        public string OneTryCrimeNumber
        {
            get { return _oneTryCrimeNumber; }
            set { Set(ref _oneTryCrimeNumber, value); }
        }
        private void InitializeCommands()
        {
            LoadCmd = new RelayCommand(Load);
            DeleteNetworkCmd = new RelayCommand(DeleteNetwork);
            UseNetworkCmd = new RelayCommand(UseNetwork);
            LoadTrainingDataCmd = new RelayCommand(LoadTrainingData);
            StartLearningCmd = new RelayCommand(StartLearning);
            LoadTestDataCmd = new RelayCommand(LoadTestData);
            OneTryCmd = new RelayCommand(OneTry);
        }

        private void LoadTestData()
        {
            var yourData = File.ReadAllLines(Consts.Consts.TrainingPath)
              .Skip(1)
              .Select(x => x.Split(','));
            
            var testModels = DeserializeTests(yourData);
            var list = testModels.ToArray();
            var newList = new List<TrainingModel>();
            var rand = new Random();
            int number = rand.Next(1000, 800000);
            for (int i = number; i < number + 2000; i++)
            {
                newList.Add(list[i]);
            }
            DataList = newList;
        }

        private IEnumerable<TrainingModel> DeserializeTests(IEnumerable<string[]> tests)
        {
            var trainingModels = new ObservableCollection<TrainingModel>();
            foreach (var t in tests)
            {
                try
                {
                    var date = DateTime.Parse(t[0]);
                    var x = double.Parse(t[t.Count() - 2].Replace(".", ","));
                    var y = double.Parse(t[t.Count() - 1].Replace(".", ","));
                    var category = t[1];
                    trainingModels.Add(new TrainingModel(date, x, y, category));
                }
                catch
                {
                    
                }
               
            }
            var a = NormalizeXy(trainingModels);
            return NormalizeDatesInTrains(a);
        }
        

        private void DeleteNetwork()
        {
            if (MessageBox.Show("Network will be deleted.R U sure? ", "", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.OK) == MessageBoxResult.Yes)
            {
                File.Delete("siec.bin");
                IsLearningDone = false;

            }
        }
        private void OneTry()
        {
            if (DataList==null)
            {
                DataList = LoadData();
            }
            OneTryCrimeNumber = string.Empty;
            int numberOfRecords = 1;
            
            double[] networkInput = new double[6];
            while (true)
            {
                Thread.Sleep(10);

                try
                {
                    int errorCount = 0;
                    for (int j = 0; j < numberOfRecords; j++)
                    {
                        Network network = Network.Load("siec.bin");
                        networkInput[0] = ((Convert.ToDouble(Width)) - _minX) / (_maxX - _minX);
                        networkInput[1] = ((Convert.ToDouble(Height)) - _minY) / (_maxY - _minY);
                        var date = new DateTime(int.Parse(Year), (int.Parse(Month)), int.Parse(Day));
                        var dayOfWeek = (int) date.DayOfWeek;
                        networkInput[2] = Convert.ToDouble((dayOfWeek / 7).ToString());
                        networkInput[3] = Convert.ToDouble(int.Parse(Month)) / 31;
                        networkInput[4] = int.Parse(Day);
                        networkInput[5] = int.Parse(Hour);




                        //Display results
                        double max = 0;


                        double t = 0;
                        int maxIndex = 0;
                        for (int i = 0; i < 38; i++)
                        {
                            t = network.Compute(networkInput)[i];
                            if (t > max)
                            {
                                max = t;
                                maxIndex = i;
                            }
                        }
                        OneTryCrimeNumber = "Found crime index: " + maxIndex.ToString();
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Use of neural network was not successful?.", "Error", MessageBoxButton.OK);
                }
                break;
            }
        }
        private void UseNetwork()
        {
            int numberOfRecords;
            Int32.TryParse(NumberOfRecords, out numberOfRecords);


            // input array
            double[] networkInput = new double[6];
            var data = DataList.ToArray();
            while (true)
            {
                Thread.Sleep(10);

                try
                {
                    int errorCount = 0;
                    for (int j = 0; j < numberOfRecords; j++)
                    {
                        Network network = Network.Load("siec.bin");
                        //networkInput[0] = data[j].NormalizedDate;
                        networkInput[0] = data[j].NormalizedX;
                        networkInput[1] = data[j].NormalizedY;
                        networkInput[2] = data[j].NormalizedDayOfWeek;
                        networkInput[3] = data[j].NormalizedMonth;
                        networkInput[4] = data[j].NormalizedDayOfMonth;
                        networkInput[5] = data[j].NormalizedHour;
                        
                        
                        

                        //Display results
                        double max = 0;
                        
                        
                        double t = 0;
                        int maxIndex = 0;
                        for (int i = 0; i < 38; i++)
                        {
                            t = network.Compute(networkInput)[i];
                            if (t > max)
                            {
                                max = t;
                                maxIndex = i;
                            }
                        }

                        if (maxIndex == data[j].TrueCrimeIndex)
                        {
                            data[j].Status = "OK";
                        }
                        else
                        {
                            data[j].Status = "NOT";
                        }
                   
                        


                       
                    }
                    double totalError = (double)errorCount / (double)numberOfRecords;
                    ErrorNumber = totalError.ToString();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Use of neural network was not successful?.", "Error", MessageBoxButton.OK);
                }
                break;
            }
        }
        private void LoadTrainingData()
        {
            var testModels = LoadData();
            DataList = testModels;
        }
        private void StartLearning()
        {
            IsNotWorking = true;
            double learningRate;
            double momentum;
            double sigmoidAlphaValue;
            int hiddenNeurons;
            int iterations;
            try
            {
                learningRate = Math.Max(0.00001, Math.Min(1, double.Parse(LearningRate)));
            }
            catch
            {
                learningRate = 0.1;
            }
            // get momentum
            try
            {
               momentum = Math.Max(0, Math.Min(0.5, double.Parse(Momentum)));
            }
            catch
            {
               momentum = 0;
            }
            // get sigmoid's alpha value
            try
            {
                sigmoidAlphaValue = Math.Max(0.001, Math.Min(50, double.Parse(SigmoidAlpha)));
            }
            catch
            {
               sigmoidAlphaValue = 2;
            }
            // get hidden neurons number
            try
            {
                hiddenNeurons = Math.Max(1, Math.Min(100, Int32.Parse(HiddenNeuronsNumber)));
            }
            catch
            {
                hiddenNeurons = 2;
            }
            // iterations
            try
            {
               iterations = Math.Max(0, int.Parse(IterationsNumber));
            }
            catch
            {
                iterations = 1000;
            }
            // run worker thread

            // number of learning samples

            //int samples = DataList.Count(); 
            int samples = 5000;

            // prepare learning data
            int numberOfRecords;
            int.TryParse(NumberOfRecords, out numberOfRecords);

            var data = DataList.ToArray();
            // input, outpu arrays
            double[][] input = new double[samples][];
            double[][] output = new double[samples][];

            for (int i = 0; i < samples; i++)
            {
                input[i] = new double[6];
                output[i] = data[i].CategoryVector;
                //input[i][0] = data[i].NormalizedDate;
                input[i][0] = data[i].NormalizedX;
                input[i][1] = data[i].NormalizedY;
                input[i][2] = data[i].NormalizedDayOfWeek;
                input[i][3] = data[i].NormalizedMonth;
                input[i][4] = data[i].NormalizedDayOfMonth;
                input[i][5] = data[i].NormalizedHour;
            }
            //needToStop = false;
            if (!File.Exists("siec.bin"))
            {
                ProgressWindow progWindow = new ProgressWindow();
                _workerThread = new Thread(()=> NetworkLearning(input, output, samples, progWindow, hiddenNeurons, sigmoidAlphaValue, learningRate, momentum, iterations));
                _workerThread.Start();
            }
            IsNotWorking = true;
        }

        private void NetworkLearning(double[][] input, double[][] output, int samples, ProgressWindow progWindow, int hiddenNeurons, double sigmoidAlpha,
            double learningRate, double momentum, int iterations)
        {
            IsLearningDone = false;
            IsNotWorking = false;
            int step, lastStep = 0;
            ActivationNetwork network = new ActivationNetwork(
            new SigmoidFunction(sigmoidAlpha), 
            6, hiddenNeurons, 39);
            BackPropagationLearning teacher = new BackPropagationLearning(network);
            // set learning rate and momentum
            teacher.LearningRate = learningRate;
            teacher.Momentum = momentum;
            // iterations
            int iteration = 1;

            progWindow.MaxIter = iterations;
            double error = 1000;
            // loop
            while (true)
            {
                //progWindow.Show();
                step = iterations / 1000;
                if (iteration > lastStep + step)
                {
                    lastStep = lastStep + step;
                    //progWindow.ProgressBar.Increment(1);
                    //progWindow.Error.Text = error.ToString();
                    //progWindow.Refresh();
                }


                // run epoch of learning procedure
                error = teacher.RunEpoch(input, output) / samples;
                ErrorNumber = error.ToString();
                // increase current iteration
                iteration++;
                // check if we need to stop
                if ((iterations != 0) && (iteration > iterations))
                {
                    progWindow.Iter = iteration;
                    break;
                }
                if (error < 0.01)
                {
                    break;
                }
            }
            //progWindow.Close();
            network.Save("siec.bin");
            IsLearningDone = true;
            //Refreshing main window after finishing
            IsNotWorking = true;
        }

        private  void Load()
        {
            IsNotWorking = false;
            IsLearningDone = true;
            if (!File.Exists("siec.bin")) IsLearningDone = false;

            SetDefaultSettings();
            IsNotWorking = true;
        }

        private void SetDefaultSettings()
        {
            LearningRate = "0.1";
            Momentum = "0";
            SigmoidAlpha = "2";
            NumberOfRecords = "100";
            HiddenNeuronsNumber = "8";
            IterationsNumber = "100";
        }


        private IEnumerable<TrainingModel> NormalizeDatesInTrains(IEnumerable<TrainingModel> trainingModels) //and NormalizeDayOfWeek
        {
            var maxDate = DateTime.MinValue;
            var minDate = DateTime.MaxValue;
            foreach (var t in trainingModels)
            {
                if (t.Dates < minDate)
                    minDate = t.Dates;
                if (t.Dates > maxDate)
                    maxDate = t.Dates;
            }
            foreach (var t in trainingModels)
            {
                t.NormalizedDate = (DateTimeToDouble(t.Dates) - DateTimeToDouble(minDate))
                                   /(DateTimeToDouble(maxDate) - DateTimeToDouble(minDate));
            }
            return trainingModels;
        }
        double _minX = -121;
        double _maxX = -123;
        double _minY = 38;
        double _maxY = 37;
        private IEnumerable<TrainingModel> NormalizeXy(IEnumerable<TrainingModel> trainingModels)
        {
            var trainingModelsWithoutBugs = trainingModels.ToList();
            trainingModelsWithoutBugs.Clear();
            foreach (var t in trainingModels)
            {
                if (t.X != -120.5 && t.Y != 90)
                {
                    trainingModelsWithoutBugs.Add(t);
                }
                else
                {
                    int a = 9;
                }
            }
            foreach (var t in trainingModelsWithoutBugs)
            {
                if (t.X < _minX)
                    _minX = t.X;
                if (t.X > _maxX)
                    _maxX = t.X;
                if (t.Y < _minY)
                    _minY = t.Y;
                if (t.Y > _maxY)
                    _maxY = t.Y;

            }
            foreach (var t in trainingModelsWithoutBugs)
            {
                t.NormalizedX = (t.X - _minX)/(_maxX - _minX);
                t.NormalizedY = (t.Y - _minY) / (_maxY - _minY);

            }

            return trainingModelsWithoutBugs;
        }

        private double DateTimeToDouble(DateTime dateTime)
        {
            return Double.Parse(dateTime.Year + dateTime.Month.ToString()
                                + dateTime.Day + dateTime.Hour
                                + dateTime.Minute);
        }
        private IEnumerable<string>LoadCrimeCategoryList()
        {
            var crimeList = new List<string>();
            var yourData = File.ReadAllLines(Consts.Consts.TrainingPath)
               .Skip(1)
               .Select(x => x.Split(','));
            foreach (var t in yourData)
            {
                if (!crimeList.Contains(t[1]))
                {
                    crimeList.Add(t[1]);
                }
            }
            return crimeList;
        }

     
        private IEnumerable<TrainingModel> LoadData()
        {
            var yourData = File.ReadAllLines(Consts.Consts.TrainingPath)
                .Skip(1)
                .Select(x => x.Split(','));

            return DeserializeTrains(yourData);

        }

        private IEnumerable<TrainingModel> DeserializeTrains(IEnumerable<string[]> tab)
        {
            var trainingModels = new ObservableCollection<TrainingModel>();
            foreach (var t in tab)
            {
                var date = DateTime.Parse(t[0]);
                var x = double.Parse(t[t.Count() - 2].Replace(".", ","));
                var y = double.Parse(t[t.Count() - 1].Replace(".", ","));
                var category = t[1];
                if (category == "LARCENY/THEFT" || category == "OTHER OFFENSES"
                    || category == "ASSAULT" || category == "NON-CRIMINAL")
                    continue;
                trainingModels.Add(new TrainingModel(date,x,y,category));
            }
            var a = NormalizeXy(trainingModels);
            return NormalizeDatesInTrains(a);
        }
    }
}