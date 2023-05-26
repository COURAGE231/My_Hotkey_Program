using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
namespace WindowsFormsApp2
{
    partial class MainForm : Form
    {

        // Константы для клавиш F2 и F3
        private const int VK_F2 = 0x71;
        private const int VK_F3 = 0x72;
        private KeyboardHook keyboardHook;
        private bool isProgramRunning = false;

        // Словарь для хранения пунктов меню и соответствующих программ
        private Dictionary<Keys, string> menuItems;

        private ComboBox hotkeyComboBox;
        private Button browseButton;
        private TextBox pathTextBox;
        private ListBox menuListBox;
        private Button addButton;
        private Button removeButton;
        private NotifyIcon notifyIcon;
        private ContextMenu trayMenu;

        public MainForm()
        {

            Text = "My Hotkey Program";
            Icon = Properties.Resources.key_icon;
            BackColor = Color.DimGray;
            // Инициализация пунктов меню
            menuItems = new Dictionary<Keys, string>();

            InitializeComponents();


            keyboardHook = new KeyboardHook();
            keyboardHook.KeyDown += KeyboardHook_KeyDown;
            MaximumSize = Size;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;


        }
        private void InitializeComponents()
        {

            hotkeyComboBox = new ComboBox();
            browseButton = new Button();
            pathTextBox = new TextBox();
            menuListBox = new ListBox();
            addButton = new Button();
            removeButton = new Button();

            // Настройка свойств элементов управления
            hotkeyComboBox.Location = new System.Drawing.Point(12, 12);
            hotkeyComboBox.Size = new System.Drawing.Size(100, 21);
            hotkeyComboBox.Items.AddRange(new object[] { "F2", "F3" });
            hotkeyComboBox.SelectedIndex = 0;
            hotkeyComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            hotkeyComboBox.TabIndex = 0;
            hotkeyComboBox.BackColor = Color.White;

            browseButton.Location = new System.Drawing.Point(324, 12);
            browseButton.Size = new System.Drawing.Size(75, 21);
            browseButton.Text = "Обзор";
            browseButton.Click += browseButton_Click;
            browseButton.BackColor = Color.White;


            pathTextBox.Location = new System.Drawing.Point(118, 12);
            pathTextBox.Size = new System.Drawing.Size(200, 20);


            menuListBox.Location = new System.Drawing.Point(12, 38);
            menuListBox.Size = new System.Drawing.Size(306, 160);
            menuListBox.MouseDoubleClick += menuListBox_MouseDoubleClick; // Добавление обработчика события MouseDoubleClick
            menuListBox.BackColor = Color.White;
            menuListBox.ForeColor = Color.Black;
            menuListBox.Font = new Font("Arial", 12);
            menuListBox.ItemHeight = 30; // Установка высоты элементов списка на 30 пикселей

            addButton.Location = new System.Drawing.Point(12, 204);
            addButton.Size = new System.Drawing.Size(147, 50);
            addButton.Text = "Добавить";
            addButton.Click += addButton_Click;
            addButton.BackColor = Color.White;
            addButton.ImageAlign = ContentAlignment.MiddleLeft;
            addButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            addButton.Image = Properties.Resources.plus_key.ToBitmap();



            removeButton.Location = new System.Drawing.Point(170, 204);
            removeButton.Size = new System.Drawing.Size(147, 50);
            removeButton.Text = "Удалить";
            removeButton.Click += removeButton_Click;
            removeButton.BackColor = Color.White;
            removeButton.ImageAlign = ContentAlignment.MiddleLeft;
            removeButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            removeButton.Image = Properties.Resources.remove_key.ToBitmap();

            // Инициализация контекстного меню для иконки в системном трее
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Включить", OnEnableClicked);
            trayMenu.MenuItems.Add("Выключить", OnExitClicked);

            notifyIcon = new NotifyIcon();
            Icon key_icon = Properties.Resources.key_icon;
            notifyIcon.Icon =  key_icon;
            notifyIcon.Text = "Ваше приложение";
            notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            notifyIcon.ContextMenu = trayMenu; // Привязка контекстного меню к иконке

            // Добавление элементов управления на форму
            Controls.Add(hotkeyComboBox);
            Controls.Add(browseButton);
            Controls.Add(pathTextBox);
            Controls.Add(menuListBox);
            Controls.Add(addButton);
            Controls.Add(removeButton);

            // Установка размера окна
            Size = new System.Drawing.Size(450, 300);
        }

        private void OnEnableClicked(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            Show();
            notifyIcon.Visible = false;
        }
        private void OnExitClicked(object sender, EventArgs e)
        {
            // Закрываем приложение
            Application.Exit();
        }
        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                notifyIcon.Visible = true;
            }
            else
            {
                base.OnFormClosing(e);
            }
        }

        private void KeyboardHook_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                if (!isProgramRunning && menuItems.ContainsKey(Keys.F2))
                {
                    isProgramRunning = true;
                    MessageBox.Show("Вызов программы для F2");
                    RunProgram(menuItems[Keys.F2]);
                    isProgramRunning = false;
                }
                else
                {
                    MessageBox.Show("Неверный пункт меню или программа уже запущена!");
                }
            }
            else if (e.KeyCode == Keys.F3)
            {
                if (!isProgramRunning && menuItems.ContainsKey(Keys.F3))
                {
                    isProgramRunning = true;
                    MessageBox.Show("Вызов программы для F3");
                    RunProgram(menuItems[Keys.F3]);
                    isProgramRunning = false;
                }
                else
                {
                    MessageBox.Show("Неверный пункт меню или программа уже запущена!");
                }
            }
        }

        private void menuListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (menuListBox.SelectedIndex != -1)
            {
                string selectedItem = menuListBox.SelectedItem.ToString();
                string hotkeyString = selectedItem.Split(':')[0].Trim();
                Keys hotkey = (Keys)Enum.Parse(typeof(Keys), hotkeyString);

                if (menuItems.ContainsKey(hotkey))
                {
                    RunProgram(menuItems[hotkey]);
                }
            }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            string filePath = BrowseFilePath();
            if (!string.IsNullOrEmpty(filePath))
            {
                pathTextBox.Text = filePath;
            }
        }

        private string BrowseFilePath()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Executable Files|*.exe";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return openFileDialog.FileName;
                }
            }
            return null;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            string path = pathTextBox.Text.Trim();
            string selectedHotkey = hotkeyComboBox.SelectedItem.ToString(); // Получение выбранной кнопки
            Keys hotkey = selectedHotkey == "F2" ? Keys.F2 : Keys.F3; // Преобразование выбранной кнопки в Keys

            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("Введите путь к программе!");
                return;
            }

            if (!menuItems.ContainsKey(hotkey))
            {
                menuItems.Add(hotkey, path);
                menuListBox.Items.Add(selectedHotkey + ": " + path);
            }
            else
            {
                MessageBox.Show("Горячая клавиша уже назначена!");
            }

            pathTextBox.Clear();
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (menuListBox.SelectedIndex != -1)
            {
                string selectedItem = menuListBox.SelectedItem.ToString();
                string hotkeyString = selectedItem.Split(':')[0].Trim();
                Keys hotkey = (Keys)Enum.Parse(typeof(Keys), hotkeyString);

                menuItems.Remove(hotkey);
                menuListBox.Items.Remove(menuListBox.SelectedItem);
            }
        }

        private void RunProgram(string path)
        {
            try
            {
                // Создаем новый процесс
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = path;

                    // Запускаем программу
                    process.Start();

                    // Ждем, пока программа завершится
                    process.WaitForExit();

                    // Выводим код завершения программы
                    MessageBox.Show("Код завершения: " + process.ExitCode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при запуске программы: " + ex.Message);
            }
        }
    }
}