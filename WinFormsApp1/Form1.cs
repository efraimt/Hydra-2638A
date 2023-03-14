namespace WinFormsApp1
{
    public partial class Form1 : Form
    {

        Fluke.Fluke2638A fluke;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            fluke = new Fluke.Fluke2638A(txtIp.Text);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var command = txtCommand.Text;
            var result  = await fluke.SendAsync(command);
            lblResult.Text = result;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await fluke.StopScanAsync();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            var count = await fluke.GetDataPointCountAsync();
            lblResult.Text = count.ToString();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            await fluke.ClearDataAsync();
        }



    }
}