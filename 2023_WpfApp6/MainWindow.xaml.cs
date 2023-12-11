using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace _2023_WpfApp6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string url = "https://data.moenv.gov.tw/api/v2/aqx_p_432?api_key=e8dd42e6-9b8b-43f8-991e-b3dee723a52d&limit=1000&sort=ImportDate%20desc&format=JSON";
        AQIdata aqidata = new AQIdata();
        List<Field> fields = new List<Field>();
        List<Record> records = new List<Record>();
        public MainWindow()
        {
            InitializeComponent();
            UrlTextBox.Text = url;
        }

        private async void GetWebDataButton_Click(object sender, RoutedEventArgs e)
        {
            ContentTextBox.Text = "正在抓取網路資料...";

            string jsontext = await FetchContentAsync(url);
            ContentTextBox.Text = jsontext;
            aqidata = JsonSerializer.Deserialize<AQIdata>(jsontext);
            fields = aqidata.fields.ToList();
            records = aqidata.records.ToList();
            StatusTextBlock.Text = $"共有 {records.Count} 筆資料";
            DisplayAQIData();
        }

        private void DisplayAQIData()
        {
            RecordDataGrid.ItemsSource = records;
        }

        private async Task<string> FetchContentAsync(string url)
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
                return $"發生錯誤: {ex.Message}";
            }
        }
    }
}