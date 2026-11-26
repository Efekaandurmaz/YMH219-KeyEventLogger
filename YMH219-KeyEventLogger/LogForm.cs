namespace KeyEventLogger;

public class LogForm : Form
{
    private readonly ListBox _lstLog;
    private readonly Label _lblStatus;
    private readonly Button _btnStart, _btnStop, _btnClear, _btnSave;
    private bool _isRecording;

    public LogForm()
    {
        Text = "Key Event Logger (Eğitsel)";
        ClientSize = new Size(520, 460);
        StartPosition = FormStartPosition.CenterScreen;
        KeyPreview = true;   // tuşları önce form yakalasın

        _btnStart = CreateButton("Başlat", 10, (s, e) => SetRecording(true));
        _btnStop = CreateButton("Durdur", 120, (s, e) => SetRecording(false));
        _btnClear = CreateButton("Temizle", 230, (s, e) => _lstLog!.Items.Clear());
        _btnSave = CreateButton("Kaydet (.txt)", 340, (s, e) => SaveToFile());

        _lblStatus = new Label
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(10, 50)
        };

        var lblInfo = new Label
        {
            Text = "Yalnızca bu pencere aktifken basılan tuşlar kaydedilir.",
            AutoSize = true,
            Location = new Point(10, 78)
        };

        _lstLog = new ListBox
        {
            SelectionMode = SelectionMode.None,
            Font = new Font("Consolas", 10),
            Location = new Point(10, 105),
            Size = new Size(500, 345),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };

        Controls.AddRange(new Control[] { _btnStart, _btnStop, _btnClear, _btnSave, _lblStatus, lblInfo, _lstLog });
        SetRecording(false);
    }

    private Button CreateButton(string text, int x, EventHandler onClick)
    {
        var btn = new Button { Text = text, Location = new Point(x, 10), Size = new Size(105, 30), TabStop = false };
        btn.Click += (s, e) =>
        {
            onClick(s, e);
            _lstLog?.Focus();   // Space/Enter butonu tetiklemesin
        };
        return btn;
    }

    private void SetRecording(bool on)
    {
        _isRecording = on;
        _btnStart.Enabled = !on;
        _btnStop.Enabled = on;
        _lblStatus.Text = on ? "● KAYIT AÇIK" : "● KAYIT KAPALI";
        _lblStatus.ForeColor = on ? Color.Green : Color.Firebrick;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        AddLog("KeyDown", e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);
        AddLog("KeyUp  ", e);
    }

    private void AddLog(string eventName, KeyEventArgs e)
    {
        if (!_isRecording) return;

        string mod = e.Modifiers != Keys.None ? $" ({e.Modifiers})" : "";
        _lstLog.Items.Add($"{DateTime.Now:HH:mm:ss.fff}  {eventName}  {e.KeyCode}{mod}");
        _lstLog.TopIndex = _lstLog.Items.Count - 1;   // en alta kaydır
    }

    private void SaveToFile()
    {
        if (_lstLog.Items.Count == 0)
        {
            MessageBox.Show("Kaydedilecek kayıt yok.", "Bilgi");
            return;
        }

        using var dlg = new SaveFileDialog
        {
            Filter = "Metin dosyası (*.txt)|*.txt",
            FileName = $"keylog_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
        };
        if (dlg.ShowDialog() != DialogResult.OK) return;

        try
        {
            var lines = _lstLog.Items.Cast<object>().Select(i => i.ToString() ?? "");
            File.WriteAllLines(dlg.FileName, lines, System.Text.Encoding.UTF8);
            MessageBox.Show("Kayıtlar dosyaya yazıldı.", "Bilgi");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Dosya kaydedilemedi: " + ex.Message, "Hata");
        }
    }
}