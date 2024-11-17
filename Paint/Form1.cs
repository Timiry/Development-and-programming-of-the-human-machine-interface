using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Paint
{
    public partial class Form1 : Form
    {


        // TO DO
        // ������� ������ � ��������� ����� ���� �� ���� ������
        // ������� ����� � ������ ���������
        // ������ ���������� � ����� ������

        private bool drawing;
        private Point lastPoint;
        private Color currentColor = Color.Black;
        private Bitmap backgroundImage;
        private int lineWidth = 2;
        private string brushShape = "Line"; // "Line", "Rectangle", "Ellipse"
        private bool eraserMode = false;
        private List<Shape> shapes = new List<Shape>();
        private Shape currentShape = null;
        public Form1()
        {
            this.Text = "My Paint";
            this.DoubleBuffered = true;
            this.MouseDown += new MouseEventHandler(MouseDownEvent);
            this.MouseMove += new MouseEventHandler(MouseMoveEvent);
            this.MouseUp += new MouseEventHandler(MouseUpEvent);
            this.Paint += new PaintEventHandler(PaintEvent);

            // �������� ����
            MenuStrip menuStrip = new MenuStrip();
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("����");
            ToolStripMenuItem colorMenu = new ToolStripMenuItem("����");
            ToolStripMenuItem loadImageMenu = new ToolStripMenuItem("��������� �����������");
            ToolStripMenuItem lineWidthMenu = new ToolStripMenuItem("������� �����");
            ToolStripMenuItem brushShapeMenu = new ToolStripMenuItem("����� �����");
            ToolStripMenuItem eraserMenu = new ToolStripMenuItem("������");
            ToolStripMenuItem undoMenu = new ToolStripMenuItem("������");
            ToolStripMenuItem saveMenu = new ToolStripMenuItem("���������");
            ToolStripMenuItem fillMenu = new ToolStripMenuItem("�������");
            ToolStripMenuItem textMenu = new ToolStripMenuItem("�������� �����");

            loadImageMenu.Click += new EventHandler(LoadImage);
            colorMenu.Click += new EventHandler(ChooseColor);
            lineWidthMenu.Click += new EventHandler(ChooseLineWidth);
            brushShapeMenu.Click += new EventHandler(ChooseBrushShape);
            eraserMenu.Click += new EventHandler(ToggleEraser);
            undoMenu.Click += new EventHandler(UndoLastAction);
            saveMenu.Click += new EventHandler(SaveImage);
            fillMenu.Click += new EventHandler(ChooseFillColor);
            textMenu.Click += new EventHandler(AddText);

            fileMenu.DropDownItems.Add(loadImageMenu);
            fileMenu.DropDownItems.Add(saveMenu);

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(lineWidthMenu);
            menuStrip.Items.Add(brushShapeMenu);
            menuStrip.Items.Add(colorMenu);
            menuStrip.Items.Add(fillMenu);
            menuStrip.Items.Add(textMenu);
            menuStrip.Items.Add(eraserMenu);
            menuStrip.Items.Add(undoMenu);

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            this.Width = 800;
            this.Height = 600;
        }

        private void MouseDownEvent(object sender, MouseEventArgs e)
        {
            drawing = true;
            lastPoint = e.Location;

            if (brushShape != "Line")
            {
                currentShape = new Shape
                {
                    StartPoint = lastPoint,
                    EndPoint = lastPoint,
                    Color = eraserMode ? this.BackColor : currentColor,
                    LineWidth = lineWidth,
                    ShapeType = brushShape
                };
            }
        }

        private void MouseMoveEvent(object sender, MouseEventArgs e)
        {
            if (drawing)
            {
                if (brushShape == "Line")
                {
                    using (Graphics g = this.CreateGraphics())
                    {
                        g.DrawLine(new Pen(currentColor, lineWidth), lastPoint, e.Location);
                    }
                }
                else if (brushShape == "Rectangle" || brushShape == "Ellipse")
                {
                    currentShape.EndPoint = e.Location;
                    this.Invalidate(); // ������������ �����
                }

                lastPoint = e.Location;
            }
        }

        private void MouseUpEvent(object sender, MouseEventArgs e)
        {
            drawing = false;

            if (currentShape != null)
            {
                shapes.Add(currentShape);
                currentShape = null;
            }
        }

        private void PaintEvent(object sender, PaintEventArgs e)
        {
            if (backgroundImage != null)
            {
                e.Graphics.DrawImage(backgroundImage, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            }

            foreach (var shape in shapes)
            {
                shape.Draw(e.Graphics);
            }

            if (currentShape != null)
            {
                currentShape.Draw(e.Graphics);
            }
        }

        private void ChooseColor(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                currentColor = colorDialog.Color;
            }
        }

        private void LoadImage(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                backgroundImage = new Bitmap(openFileDialog.FileName);
                this.Invalidate(); // ������������ �����, ����� ���������� ����� �����������
            }
        }

        private void ChooseLineWidth(object sender, EventArgs e)
        {
            using (Form lineWidthForm = new Form())
            {
                lineWidthForm.Height = 130;
                lineWidthForm.Width = 400;
                lineWidthForm.Text = "����� ������� �����";
                NumericUpDown numericUpDown = new NumericUpDown
                {
                    Minimum = 1,
                    Maximum = 20,
                    Value = lineWidth,
                    Dock = DockStyle.Fill
                };
                Button okButton = new Button
                {
                    Text = "OK",
                    Dock = DockStyle.Fill
                };
                okButton.Click += (s, ev) =>
                {
                    lineWidth = (int)numericUpDown.Value;
                    lineWidthForm.Close();
                };

                lineWidthForm.Controls.Add(numericUpDown);
                lineWidthForm.Controls.Add(okButton);
                lineWidthForm.ShowDialog();
            }
        }

        private void ChooseBrushShape(object sender, EventArgs e)
        {
            using (Form brushShapeForm = new Form())
            {
                brushShapeForm.Height = 130;
                brushShapeForm.Width = 400;
                brushShapeForm.Text = "����� ����� �����";
                ComboBox comboBox = new ComboBox
                {
                    Dock = DockStyle.Fill,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                comboBox.Items.AddRange(new string[] { "Line", "Rectangle", "Ellipse" });
                comboBox.SelectedItem = brushShape;

                Button okButton = new Button
                {
                    Text = "OK",
                    Dock = DockStyle.Fill
                };
                okButton.Click += (s, ev) =>
                {
                    brushShape = comboBox.SelectedItem.ToString();
                    brushShapeForm.Close();
                };

                brushShapeForm.Controls.Add(comboBox);
                brushShapeForm.Controls.Add(okButton);
                brushShapeForm.ShowDialog();
            }
        }

        private void ToggleEraser(object sender, EventArgs e)
        {
            eraserMode = !eraserMode;
            (sender as ToolStripMenuItem).Text = eraserMode ? "����� ���������" : "������";
        }

        private void UndoLastAction(object sender, EventArgs e)
        {
            if (shapes.Count > 0)
            {
                shapes.RemoveAt(shapes.Count - 1);
                this.Invalidate(); // ������������ �����
            }
        }

        private void SaveImage(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap bitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(this.BackColor);
                    if (backgroundImage != null)
                    {
                        g.DrawImage(backgroundImage, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
                    }
                    foreach (var shape in shapes)
                    {
                        shape.Draw(g);
                    }
                }
                bitmap.Save(saveFileDialog.FileName);
                bitmap.Dispose();
            }
        }

        private void ChooseFillColor(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                currentColor = colorDialog.Color;
                if (currentShape != null && (currentShape.ShapeType == "Rectangle" || currentShape.ShapeType == "Ellipse"))
                {
                    currentShape.FillColor = currentColor; // ���������� ���� �������
                    this.Invalidate(); // ������������ �����
                }
            }
        }

        private void AddText(object sender, EventArgs e)
        {
            using (Form textForm = new Form())
            {
                textForm.Text = "�������� �����";
                TextBox textBox = new TextBox { Dock = DockStyle.Fill };
                Button okButton = new Button { Text = "OK", Dock = DockStyle.Bottom };

                okButton.Click += (s, ev) =>
                {
                    string text = textBox.Text;
                    if (!string.IsNullOrEmpty(text))
                    {
                        shapes.Add(new TextShape
                        {
                            Text = text,
                            Position = lastPoint,
                            Color = currentColor,
                            Font = new Font("Arial", 12)
                        });
                        this.Invalidate(); // ������������ �����
                    }
                    textForm.Close();
                };

                textForm.Controls.Add(textBox);
                textForm.Controls.Add(okButton);
                textForm.ShowDialog();
            }
        }

        public class Shape
        {
            public Point StartPoint { get; set; }
            public Point EndPoint { get; set; }
            public Color Color { get; set; }
            public Color FillColor { get; set; }
            public int LineWidth { get; set; }
            public string ShapeType { get; set; }

            public void Draw(Graphics g)
            {
                using (Pen pen = new Pen(Color, LineWidth))
                {
                    if (ShapeType == "Line")
                    {
                        g.DrawLine(pen, StartPoint, EndPoint);
                    }
                    else if (ShapeType == "Rectangle")
                    {
                        g.FillRectangle(new SolidBrush(FillColor), Math.Min(StartPoint.X, EndPoint.X), Math.Min(StartPoint.Y, EndPoint.Y),
                            Math.Abs(StartPoint.X - EndPoint.X), Math.Abs(StartPoint.Y - EndPoint.Y));
                        g.DrawRectangle(pen, Math.Min(StartPoint.X, EndPoint.X), Math.Min(StartPoint.Y, EndPoint.Y),
                            Math.Abs(StartPoint.X - EndPoint.X), Math.Abs(StartPoint.Y - EndPoint.Y));
                    }
                    else if (ShapeType == "Ellipse")
                    {
                        g.FillEllipse(new SolidBrush(FillColor), Math.Min(StartPoint.X, EndPoint.X), Math.Min(StartPoint.Y, EndPoint.Y),
                            Math.Abs(StartPoint.X - EndPoint.X), Math.Abs(StartPoint.Y - EndPoint.Y));
                        g.DrawEllipse(pen, Math.Min(StartPoint.X, EndPoint.X), Math.Min(StartPoint.Y, EndPoint.Y),
                            Math.Abs(StartPoint.X - EndPoint.X), Math.Abs(StartPoint.Y - EndPoint.Y));
                    }
                }
            }
        }

        public class TextShape : Shape
        {
            public string Text { get; set; }
            public Point Position { get; set; }

            public Font Font { get; set; }

            public new void Draw(Graphics g)
            {
                using (Brush brush = new SolidBrush(Color))
                {
                    g.DrawString(Text, Font, brush, Position);
                }
            }
        }
    }
}
