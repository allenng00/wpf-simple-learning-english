using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LearningEnglishSimpleGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constant values
        const int CountQuestion = 10;
        const int CountQuestionMax = 30;
        const int TimeRemain = 30;
        #endregion

        #region Attributes
        private Random _rng = new Random();
        private System.Timers.Timer _timer;
        // Mảng lưu tên file
        private string[] _images = new string[] {
            "Badminton.jpg",
            "Basketball.jpg",
            "Brush.jpg",
            "Cat.jpg",
            "City.jpg",
            "Clock.jpg",
            "Cowboy.jpg",
            "Cry.jpg",
            "Dog.jpg",
            "Eye.jpg",
            "Happy.jpg",
            "Hot Air Ballon.jpg",
            "Ice.jpg",
            "KiwiFruit.jpg",
            "Noodle.jpg",
            "Pea.jpg",
            "Picture.jpg",
            "Plane.jpg",
            "Rapper.jpg",
            "River.jpg",
            "Run.jpg",
            "Sea.jpg",
            "Skirt.jpg",
            "Sky.jpg",
            "Snake.jpg",
            "Sunflower.jpg",
            "Tie.jpg",
            "Volleyball.jpg",
            "Water.jpg",
            "Window.jpg"
        };
        // Mảng các đáp án của câu hỏi
        private Tuple<string, string, string>[] _answers = {
            Tuple.Create("Badminton", "Marathon", "1" ),
            Tuple.Create("Basket", "Basketball", "2"),
            Tuple.Create("Brush", "Bruss", "1"),
            Tuple.Create("Cat", "Fat", "1"),
            Tuple.Create("City", "Country", "1"),
            Tuple.Create("Lock", "Clock", "2"),
            Tuple.Create("Cowbell", "Cowboy", "2"),
            Tuple.Create("Cry", "Dry", "1"),
            Tuple.Create("Dog", "God", "1"),
            Tuple.Create("Eye", "I", "1"),
            Tuple.Create("Happy", "Sad", "1"),
            Tuple.Create("Hot Air Ballon", "Colorful", "2"),
            Tuple.Create("Eye", "Ice", "2"),
            Tuple.Create("Jackfruit", "KiwiFruit", "2"),
            Tuple.Create("Moodle", "Noodle", "2"),
            Tuple.Create("Pea", "Peace", "1"),
            Tuple.Create("Photo", "Picture", "2"),
            Tuple.Create("Plan", "Plane", "2"),
            Tuple.Create("Rapper", "Teacher", "1"),
            Tuple.Create("Driver", "River", "2"),
            Tuple.Create("Ruin", "Run", "2"),
            Tuple.Create("Sea", "See", "1"),
            Tuple.Create("Skirt", "Shirt", "1"),
            Tuple.Create("Sky", "Strive", "1"),
            Tuple.Create("Shake", "Snake", "2"),
            Tuple.Create("Daisy", "Sunflower", "2"),
            Tuple.Create("Tie", "Tired", "1"),
            Tuple.Create("Volleyball", "Valley", "1"),
            Tuple.Create("Water", "Matter", "1"),
            Tuple.Create("Door", "Window", "2")
        };
        // Map để đánh dấu các index đã được random
        private Dictionary<int, int> _questions_index_map = new Dictionary<int, int>();
        // Mả chứa các index sau random
        private int[] _questions_index = new int[CountQuestionMax];
        // Biến lưu số lượt chơi
        private int _turns = CountQuestion;
        // Biến lưu điểm số
        private int _scores = 0;
        // Biến lưu thời gian trả lời mỗi câu hỏi
        private int _timeremain = TimeRemain;
        // Biến lưu chỉ số câu hỏi và đáp án tương ứng để hiển thị
        private int _index = 0;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            // Chuẩn bị câu hỏi, xáo trộn bộ đề
            PrepareQuestion();
            ShowQuestion();

            // Xuất điểm 0 vào đầu trò chơi
            Dispatcher.Invoke(() =>
            {
                scoreTextBlock.Text = _scores.ToString();
            });

            // Đếm thời gian, tới hạn tự đổi ảnh
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();

        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            Dispatcher.Invoke(() =>
            {
                timerTextBlock.Text = _timeremain.ToString();
            });

            _timeremain = _timeremain - 1;

            //Nếu hết thời gian hiển thị 1 đề
            if (_timeremain == -1)
            {
                // Gán lại bộ đếm bằng thơi gian của câu hỏi (=30)
                _timeremain = TimeRemain;

                if (_turns > 0)
                {
                    ShowQuestion();
                    _turns = _turns - 1;
                }
                // Nếu hết lượt chơi
                else
                {
                    _timer.Stop();

                    // Ask whether player want to continue or quit
                    Dispatcher.Invoke(() =>
                    {
                        var result = MessageBox.Show("Bạn có muốn tiếp tục chơi không", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        // Nếu chọn Yes, thông báo đã chọn tiếp tục và khởi tạo lại lần chơi mới
                        if (result == MessageBoxResult.Yes)
                        {
                            MessageBox.Show("Bạn đã chọn tiếp tục", "Thông báo", MessageBoxButton.OK);
                            ResetData();
                            ShowQuestion();
                            scoreTextBlock.Text = _scores.ToString();
                            _timer.Start();
                        }
                        else // Ngượcc lại, thông báo và tắt ứng dụng
                        {
                            MessageBox.Show("Bạn đã chọn kết thúc trò chơi\nCảm ơn đã sử dụng ứng dụng!!", "Thông báo", MessageBoxButton.OK);
                            Application.Current.Shutdown();
                        }
                    });

                }
            }
            else
            {
                //Do nothing
            }
        }
        /*
            Hàm reset dữ liệu để bắt đầu lần chơi mới
         */
        private void ResetData()
        {
            if (_index == CountQuestionMax)
            {
                _index = 0;
                _questions_index_map.Clear();
                PrepareQuestion();
            }
            else
            {
                // Do nothing
            }

            _turns = CountQuestion;
            _scores = 0;

            Dispatcher.Invoke(() =>
            {
                scoreTextBlock.Text = String.Empty;
            });

            answer01Button.IsEnabled = true;
            answer02Button.IsEnabled = true;
        }
        /*
            Hàm chuẩn bị bộ đề cho trò chơi (Random 3 bộ mỗi khi được gọi)
         */
        private void PrepareQuestion()
        {
            int temp;

            for (int pos = 0; pos < CountQuestionMax; pos++)
            {
                do
                {
                    temp = _rng.Next(CountQuestionMax);
                }
                while (_questions_index_map.ContainsKey(temp) == true);

                _questions_index_map.Add(temp, 1);
                _questions_index[pos] = temp;
            }

        }
        /*
            Hàm hiển thị câu hỏi
        */
        private void ShowQuestion()
        {
            int pos = _questions_index[_index];

            var bitmap =
               new BitmapImage(
                   new Uri(
                       $"Images/{_images[pos]}",
                       UriKind.Relative)
                   );

            questionImage.Source = bitmap;
            countquestionLabel.Content = "Question " + (_index % CountQuestion + 1).ToString() + "/10";
            answer01Button.Content = _answers[pos].Item1;
            answer02Button.Content = _answers[pos].Item2;

            _index = _index + 1;
        }
        /*
            Hàm xử lý khi click vào answer01Button
         */
        private void answer01Button_Click(object sender, RoutedEventArgs e)
        {
            _turns = _turns - 1;

            // Nếu sự lựa chọn trùng khớp đáp án
            if (_answers[_questions_index[_index - 1]].Item3 == "1")
            {
                _scores = _scores + 1;

                this.Dispatcher.Invoke(() =>
                {
                    scoreTextBlock.Text = _scores.ToString();
                });
            }
            else
            {
                //Do nothing
            }

            // Nếu hết lượt chơi
            if (_turns == 0)
            {
                this.answer02Button.IsEnabled = false;
                this.answer01Button.IsEnabled = false;
                _timeremain = 1;
            }
            else
            {
                ShowQuestion();
                _timeremain = TimeRemain;
            }

        }
        /*
           Hàm xử lý khi click vào answer02Button
        */
        private void answer02Button_Click(object sender, RoutedEventArgs e)
        {
            _turns = _turns - 1;

            // Nếu sự lựa chọn trùng khớp đáp án
            if (_answers[_questions_index[_index - 1]].Item3 == "2")
            {
                _scores = _scores + 1;

                this.Dispatcher.Invoke(() =>
                {
                    scoreTextBlock.Text = _scores.ToString();
                });
            }
            else
            {
                //Do nothing
            }

            // Nếu hết lượt chơi
            if (_turns == 0)
            {
                this.answer01Button.IsEnabled = false;
                this.answer02Button.IsEnabled = false;
                _timeremain = 1;
            }
            else
            {
                ShowQuestion();
                _timeremain = TimeRemain;
            }

        }
    }

}
