using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace _2023_WpfApp6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        string defaultURL = "https://data.moenv.gov.tw/api/v2/aqx_p_432?api_key=e8dd42e6-9b8b-43f8-991e-b3dee723a52d&limit=1000&sort=ImportDate desc&format=JSON";
        AQIData aqidata = new AQIData();
        List<Field> fields = new List<Field>();
        List<Record> records = new List<Record>();
        public MainWindow()
        {
            InitializeComponent();
            UrlTextBox.Text = defaultURL;
        }

        private async void FetchDataButton_Click(object sender, RoutedEventArgs e)
        {
            ContentTextBox.Text = "抓取網路中...";
            string jsonData = await FetchWebDataAsync(defaultURL);
            ContentTextBox.Text = jsonData;

            aqidata = JsonSerializer.Deserialize<AQIData>(jsonData);

            fields = aqidata.fields.ToList();
            records = aqidata.records.ToList();
            StatusTextBlock.Text = $"共有 {fields.Count}個欄位， {records.Count} 筆記錄";

            DisplayAQIData();
        }

        private void DisplayAQIData()
        {
            RecordDataGrid.ItemsSource = records;

            Record record = records[1];
            DataWrapPanel.Children.Clear();

            foreach (Field field in fields)
            {
                var propertyInfo = record.GetType().GetProperty(field.id);
                if (propertyInfo!= null)
                {
                    var value = propertyInfo.GetValue(record) as string;
                    if (double.TryParse(value, out double v))
                    {
                        CheckBox cb = new CheckBox
                        {
                            Content = field.info.label,
                            Tag = field.id,
                            Margin = new Thickness(3),
                            FontSize = 14,
                            FontWeight = FontWeights.Bold,
                            Width = 120
                        };
                        DataWrapPanel.Children.Add(cb);
                    }
                }
            }
        }

        private async Task<string> FetchWebDataAsync(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    return await client.GetStringAsync(url);
                }
            }
            catch (Exception ex)
            {
                return $"錯誤：{ex.Message}";
            }
        }
    }
}