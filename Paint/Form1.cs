using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using static Paint.Form1;

namespace Paint
{
    public partial class Form1 : Form
    {
        private bool isDrawing = false;
        private bool isDrawMode = true;
        private bool isSelectMode = false;
        private bool isSelecting = false;
        private bool isDragging = false;
        private bool isPickingColor = false;
        private Point lastPoint;
        private Color color1 = Color.Black;
        private Color color2 = Color.White;
        private Bitmap canvas;
        private int lineWidth = 2;
        private string brushShape = "Карандаш";
        private bool eraserMode = false;
        private List<Shape> shapes = new List<Shape>();
        private Shape selectedShape = null;
        private Point selectionOffset;
        private LinkedList<Bitmap> undoStack = new LinkedList<Bitmap>();
        private bool isMovingText = false;
        private Rectangle selectionRectangle;
        private Bitmap selectedContent;
        private Bitmap clipboardImage;

        public Form1()
        {
            //InitializeComponent();
            this.Text = "Рисовальщик";
            this.DoubleBuffered = true;
            this.MouseDown += MouseDownEvent;
            this.MouseMove += MouseMoveEvent;
            this.MouseUp += MouseUpEvent;
            this.Paint += PaintEvent;
            this.MouseClick += MouseClickEvent;
            // Настройка холста
            canvas = new Bitmap(1200, 800);
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.White);
            }

            InitializeMenu();
            this.Width = 1300;
            this.Height = 900;
        }

        private void InitializeMenu()
        {
            // Создание меню
            MenuStrip menuStrip = new MenuStrip();
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("Файл");
            ToolStripMenuItem color1Menu = new ToolStripMenuItem("Цвет 1");
            ToolStripMenuItem color2Menu = new ToolStripMenuItem("Цвет 2");
            ToolStripMenuItem loadImageMenu = new ToolStripMenuItem("Открыть");
            ToolStripMenuItem lineWidthMenu = new ToolStripMenuItem("Толщина линии");
            ToolStripMenuItem brushShapeMenu = new ToolStripMenuItem("Форма кисти");
            ToolStripMenuItem shapesMenu = new ToolStripMenuItem("Вставить фигуру");
            ToolStripMenuItem eraserMenu = new ToolStripMenuItem("Ластик");
            ToolStripMenuItem undoMenu = new ToolStripMenuItem("Отмена");
            ToolStripMenuItem saveMenu = new ToolStripMenuItem("Сохранить");
            ToolStripMenuItem clearMenu = new ToolStripMenuItem("Очистить холст");
            ToolStripMenuItem textMenu = new ToolStripMenuItem("Добавить текст");
            ToolStripMenuItem selectMenu = new ToolStripMenuItem("Выделить");
            ToolStripMenuItem copyMenu = new ToolStripMenuItem("Копировать");
            ToolStripMenuItem pasteMenu = new ToolStripMenuItem("Вставить");
            ToolStripMenuItem eyedropperMenu = new ToolStripMenuItem("Пипетка");

            loadImageMenu.Click += LoadImage;
            color1Menu.Click += ChooseColor1;
            color2Menu.Click += ChooseColor2;
            lineWidthMenu.Click += ChooseLineWidth;
            brushShapeMenu.Click += ChooseBrushShape;
            shapesMenu.Click += ChooseShape;
            eraserMenu.Click += ToggleEraser;
            undoMenu.Click += UndoLastAction;
            saveMenu.Click += SaveImage;
            clearMenu.Click += ClearCanvas;
            textMenu.Click += AddText;
            selectMenu.Click += new EventHandler(SelectArea);
            copyMenu.Click += new EventHandler(CopyToClipboard);
            pasteMenu.Click += new EventHandler(PasteFromClipboard);
            eyedropperMenu.Click += new EventHandler(ActivateEyedropper);

            fileMenu.DropDownItems.Add(loadImageMenu);
            fileMenu.DropDownItems.Add(saveMenu);

            //selectMenu.DropDownItems.Add(copyMenu);
            //selectMenu.DropDownItems.Add(pasteMenu);

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(lineWidthMenu);
            menuStrip.Items.Add(brushShapeMenu);
            menuStrip.Items.Add(shapesMenu);
            menuStrip.Items.Add(eyedropperMenu);
            menuStrip.Items.Add(color1Menu);
            menuStrip.Items.Add(color2Menu);
            menuStrip.Items.Add(eraserMenu);
            menuStrip.Items.Add(undoMenu);
            menuStrip.Items.Add(clearMenu);
            menuStrip.Items.Add(textMenu);
            menuStrip.Items.Add(selectMenu);
          

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private void MouseDownEvent(object sender, MouseEventArgs e)
        {
            SaveUndoState();
            
            lastPoint = e.Location;

            if (isDrawMode) isDrawing = true;
            if (isSelectMode) isSelecting = true;

            if (e.Button == MouseButtons.Left && selectionRectangle.Contains(e.Location))
            {
                isDragging = true;
                selectionOffset = new Point(e.Location.X - selectionRectangle.Location.X, e.Location.Y - selectionRectangle.Location.Y);
            }

            if (!isDragging)
            {
                if (eraserMode)
                {
                    brushShape = "Карандаш";
                }

                if (brushShape == "Select" && selectedShape != null)
                {
                    if (selectedShape is TextShape textShape && textShape.Contains(e.Location))
                    {
                        isMovingText = true;
                        selectionOffset = new Point(e.Location.X - textShape.Position.X, e.Location.Y - textShape.Position.Y);
                    }
                }
                else if (brushShape == "Заливка")
                {
                    foreach (var shape in shapes)
                    {
                        if (shape.Contains(e.Location))
                        {
                            shape.FillColor = color1;
                            return;
                        }
                    }
                    PerformFloodFill(e.Location);
                }
                else if (brushShape != "Карандаш")
                {
                    selectedShape = new Shape
                    {
                        StartPoint = lastPoint,
                        EndPoint = lastPoint,
                        Color = eraserMode ? this.BackColor : color1,
                        LineWidth = lineWidth,
                        ShapeType = brushShape
                    };
                }
            }

            this.Invalidate();
        }

        private void MouseMoveEvent(object sender, MouseEventArgs e)
        {
            if (selectionRectangle.Contains(e.Location))
            {
                this.Cursor = Cursors.Hand; // Курсор для перемещения
            }
            else if (isSelectMode)
            {
                this.Cursor = Cursors.Cross;
            }
            else if (isPickingColor)
            {
                this.Cursor = Cursors.PanNorth;
            }
            else
            {
                this.Cursor = Cursors.Default;
            } 


            if (isDrawing)
            {
                if (brushShape == "Карандаш")
                {
                    using (Graphics g = Graphics.FromImage(canvas))
                    {
                        g.DrawLine(new Pen(eraserMode ? color2 : color1, lineWidth), lastPoint, e.Location);
                    }
                }
                else if (brushShape == "Увеличитель яркости")
                {
                    // Увеличение яркости пикселей
                    IncreaseBrightness(e.Location);
                }
                else if (brushShape == "Уменьшитель яркости")
                {
                    // Увеличение яркости пикселей
                    ReduceBrightness(e.Location);
                }
                else if (new string[] { "Линия", "Прямоугольник", "Эллипс", "Треугольник", "Ромб", "Шестиугольник" }.Contains(brushShape))
                {
                    selectedShape.EndPoint = e.Location;
                }
                if (eraserMode)
                {
                    // Проверяем, не попадает ли точка на текстовые фигуры
                    for (int i = shapes.Count - 1; i >= 0; i--)
                    {
                        if (shapes[i].Contains(e.Location))
                        {
                            if (shapes[i].Equals(selectedShape)) selectedShape = null;
                            shapes.RemoveAt(i); // Удаляем текст, если он попадает под ластик
                        }
                    }
                }
                lastPoint = e.Location; // Обновление стартовой точки для следующего движения
            }
            else if (isDragging)
            {
                // Перемещение выделенного прямоугольника
                selectionRectangle.Location = new Point(e.Location.X - selectionOffset.X, e.Location.Y - selectionOffset.Y);

            }
            else if (isSelecting)
            {
                selectionRectangle = new Rectangle(
                    Math.Min(lastPoint.X, e.X),
                    Math.Min(lastPoint.Y, e.Y),
                    Math.Abs(lastPoint.X - e.X),
                    Math.Abs(lastPoint.Y - e.Y)
                );
            }

            if (isMovingText && selectedShape is TextShape textShape)
            {
                textShape.Position = new Point(e.Location.X - selectionOffset.X, e.Location.Y - selectionOffset.Y);
            }

            //lastPoint = e.Location; // Обновление стартовой точки для следующего движения
            this.Invalidate(); // Перерисовать форму
        }

        private void MouseClickEvent(object sender, MouseEventArgs e)
        {
            if (!selectionRectangle.Contains(e.Location))
            {
                selectionRectangle = Rectangle.Empty; // Сброс выделенной области
                selectedContent = null;
                this.Invalidate();
            }

            if (isPickingColor)
            {
                // Получаем цвет пикселя под курсором
                Bitmap bitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
                this.DrawToBitmap(bitmap, new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height));

                // Получаем цвет пикселя по координатам мыши
                color1 = bitmap.GetPixel(e.X, e.Y);

                // Выход из режима пипетки
                isPickingColor = false;
                this.Cursor = Cursors.Default; // Возвращаем курсор к стандартному
                //MessageBox.Show($"Выбранный цвет: {color1}"); // Выводим выбранный цвет
            }
        }

        private void MouseUpEvent(object sender, MouseEventArgs e)
        {
            isDrawing = false;
            isMovingText = false;
            isDragging = false;

            if (isSelecting)
            {
                isSelecting = false;

                // Копирование содержимого выделенной области
                if (!selectionRectangle.IsEmpty)
                {
                    selectedContent = new Bitmap(selectionRectangle.Width, selectionRectangle.Height);
                    using (Graphics g = Graphics.FromImage(selectedContent))
                    {
                        g.DrawImage(canvas, new Rectangle(0, 0, selectedContent.Width, selectedContent.Height), selectionRectangle, GraphicsUnit.Pixel);
                    }
                }
            }

            if (!eraserMode && selectedShape != null && !shapes.Contains(selectedShape))
            {
                shapes.Add(selectedShape);
            }


            this.Invalidate();
        }

        private void PaintEvent(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(canvas, 0, 0);

            using (Graphics g = Graphics.FromImage(canvas))
            {
                foreach (var shape in shapes)
                {
                    shape.Draw(g);
                }

                if (selectedShape != null)
                {
                    selectedShape.Draw(e.Graphics);
                }

                if (selectionRectangle != Rectangle.Empty)
                {
                    using (Pen dashedPen = new Pen(Color.Black) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot })
                    {
                        e.Graphics.DrawRectangle(dashedPen, selectionRectangle);
                    }
                }

                if (selectedContent != null)
                {
                    g.DrawImage(selectedContent, selectionRectangle.Location);
                }
            }
        }

        private void ActivateEyedropper(object sender, EventArgs e)
        {
            isPickingColor = true;
            this.Cursor = Cursors.PanNorth;
        }

        private void IncreaseBrightness(Point location)
        {
            if (canvas != null)
            {
                for (int x = location.X - lineWidth / 2; x <= location.X + lineWidth / 2; x++)
                {
                    for (int y = location.Y - lineWidth / 2; y <= location.Y + lineWidth / 2; y++)
                    {
                        if (x >= 0 && x < canvas.Width && y >= 0 && y < canvas.Height)
                        {
                            Color pixelColor = canvas.GetPixel(x, y);
                            // Увеличиваем яркость
                            int r = Math.Clamp(pixelColor.R + 10, 0, 255);
                            int g = Math.Clamp(pixelColor.G + 10, 0, 255);
                            int b = Math.Clamp(pixelColor.B + 10, 0, 255);
                            Color brightenedColor = Color.FromArgb(pixelColor.A, r, g, b);
                            canvas.SetPixel(x, y, brightenedColor);
                        }
                    }
                }
                this.Invalidate(); // Перерисовать форму
            }
        }

        private void ReduceBrightness(Point location)
        {
            if (canvas != null)
            {
                for (int x = location.X - lineWidth / 2; x <= location.X + lineWidth / 2; x++)
                {
                    for (int y = location.Y - lineWidth / 2; y <= location.Y + lineWidth / 2; y++)
                    {
                        if (x >= 0 && x < canvas.Width && y >= 0 && y < canvas.Height)
                        {
                            Color pixelColor = canvas.GetPixel(x, y);
                            // Уменьшаем яркость
                            int r = Math.Clamp(pixelColor.R - 10, 0, 255);
                            int g = Math.Clamp(pixelColor.G - 10, 0, 255);
                            int b = Math.Clamp(pixelColor.B - 10, 0, 255);
                            Color brightenedColor = Color.FromArgb(pixelColor.A, r, g, b);
                            canvas.SetPixel(x, y, brightenedColor);
                        }
                    }
                }
                this.Invalidate(); // Перерисовать форму
            }
        }

        private void PerformFloodFill(Point start)
        {
            Color targetColor = canvas.GetPixel(start.X, start.Y);
            if (targetColor == (eraserMode ? color2 : color1)) return;

            Queue<Point> queue = new Queue<Point>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                Point current = queue.Dequeue();
                if (current.X < 0 || current.X >= canvas.Width || current.Y < 0 || current.Y >= canvas.Height) continue;
                if (canvas.GetPixel(current.X, current.Y) != targetColor) continue;

                canvas.SetPixel(current.X, current.Y, eraserMode ? color2 : color1);
                if (!queue.Contains(new Point(current.X - 1, current.Y))) 
                {
                    queue.Enqueue(new Point(current.X - 1, current.Y));
                }
                if (!queue.Contains(new Point(current.X + 1, current.Y)))
                {
                    queue.Enqueue(new Point(current.X + 1, current.Y));
                }
                if (!queue.Contains(new Point(current.X, current.Y - 1)))
                {
                    queue.Enqueue(new Point(current.X, current.Y - 1));
                }
                if (!queue.Contains(new Point(current.X, current.Y + 1)))
                {
                    queue.Enqueue(new Point(current.X, current.Y + 1));
                }
            }

            this.Invalidate();
        }

        private void SaveUndoState()
        {
            Bitmap copy = new Bitmap(canvas);
            undoStack.AddLast(copy);
            if (undoStack.Count > 30) {  }
        }

        private void UndoLastAction(object sender, EventArgs e)
        {
            if (undoStack.Count > 0)
            {
                canvas = undoStack.Last();
                undoStack.RemoveLast();
                this.Invalidate();
            }
        }

        private void ChooseColor1(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                color1 = dialog.Color;
            }
        }

        private void ChooseColor2(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                color2 = dialog.Color;
            }
        }

        private void ClearCanvas(object sender, EventArgs e)
        {
            shapes.Clear();
            selectedShape = null;
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.White);
            }
            this.Invalidate();
        }

        private void LoadImage(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (Bitmap loadedImage = new Bitmap(openFileDialog.FileName))
                {
                    using (Graphics g = Graphics.FromImage(canvas))
                    {
                        g.DrawImage(loadedImage, 0, 0, canvas.Width, canvas.Height);
                    }
                }
                this.Invalidate();
            }
        }

        private void ChooseLineWidth(object sender, EventArgs e)
        {
            using (Form lineWidthForm = new Form())
            {
                lineWidthForm.Height = 130;
                lineWidthForm.Width = 400;
                lineWidthForm.Text = "Выбор толщины линии";
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
                    Dock = DockStyle.Bottom
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
            isSelectMode = false;
            isDrawMode = true;
            this.Cursor = Cursors.Default;
            using (Form brushShapeForm = new Form())
            {
                brushShapeForm.Height = 130;
                brushShapeForm.Width = 400;
                brushShapeForm.Text = "Выбор формы кисти";
                ComboBox comboBox = new ComboBox
                {
                    Dock = DockStyle.Fill,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                comboBox.Items.AddRange(new string[] { "Карандаш", "Заливка", "Увеличитель яркости", "Уменьшитель яркости" });
                comboBox.SelectedItem = brushShape;

                Button okButton = new Button
                {
                    Text = "OK",
                    Dock = DockStyle.Bottom
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

        private void ChooseShape(object sender, EventArgs e)
        {
            isSelectMode = false;
            isDrawMode = true;
            this.Cursor = Cursors.Default;
            using (Form shapeForm = new Form())
            {
                shapeForm.Height = 130;
                shapeForm.Width = 400;
                shapeForm.Text = "Выбор фигуры";
                ComboBox comboBox = new ComboBox
                {
                    Dock = DockStyle.Fill,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                comboBox.Items.AddRange(new string[] { "Линия", "Прямоугольник", "Эллипс", "Треугольник", "Ромб", "Шестиугольник" });
                

                Button okButton = new Button
                {
                    Text = "OK",
                    Dock = DockStyle.Bottom
                };
                okButton.Click += (s, ev) =>
                {
                    brushShape = comboBox.SelectedItem.ToString();
                    shapeForm.Close();
                };

                shapeForm.Controls.Add(comboBox);
                shapeForm.Controls.Add(okButton);
                shapeForm.ShowDialog();
            }
        }

        private void SelectArea(object sender, EventArgs e)
        {
            // Начало выделения
            isDrawMode = false;
            isSelectMode = true;
            this.Cursor = Cursors.Cross; // Изменяем курсор для выделения
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.C)) // Ctrl+C
            {
                CopyToClipboard(this, EventArgs.Empty);
                return true; // Указываем, что мы обработали это событие
            }
            else if (keyData == (Keys.Control | Keys.V)) // Ctrl+V
            {
                PasteFromClipboard(this, EventArgs.Empty);
                return true; // Указываем, что мы обработали это событие
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void CopyToClipboard(object sender, EventArgs e)
        {
            if (selectionRectangle != Rectangle.Empty)
            {
                clipboardImage = new Bitmap(selectionRectangle.Width, selectionRectangle.Height);
                using (Graphics g = Graphics.FromImage(clipboardImage))
                {
                    g.DrawImage(canvas, new Rectangle(0, 0, clipboardImage.Width, clipboardImage.Height),
                        selectionRectangle, GraphicsUnit.Pixel);
                }
            }
        }

        private void PasteFromClipboard(object sender, EventArgs e)
        {
            if (clipboardImage != null)
            {
                //using (Graphics g = Graphics.FromImage(canvas))
                //{
                //    g.DrawImage(clipboardImage, 0, 28);
                //}

                selectedContent = new Bitmap(clipboardImage);
                selectionRectangle = new Rectangle(new Point(0, 28), clipboardImage.Size);
                this.Invalidate(); // Перерисовать форму
            }
        }

        private void ToggleEraser(object sender, EventArgs e)
        {
            isSelectMode = false;
            isDrawMode = true;
            eraserMode = !eraserMode;
            (sender as ToolStripMenuItem).Text = eraserMode ? "Режим рисования" : "Ластик";
        }

        private void SaveImage(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (Graphics g = Graphics.FromImage(canvas))
                {
                    // Отрисовываем все фигуры
                    foreach (var shape in shapes)
                    {
                        shape.Draw(g); // Рисуем каждую фигуру на canvas
                    }
                }

                // Сохраняем canvas в выбранный файл
                canvas.Save(saveFileDialog.FileName, ImageFormat.Png);
            }
        }

        private void AddText(object sender, EventArgs e)
        {
            using (Form textForm = new Form())
            {
                textForm.Text = "Добавить текст";
                Font choosedFont = new Font("Arial", 12);
                TextBox textBox = new TextBox { Dock = DockStyle.Fill };
                Button okButton = new Button { Text = "OK", Dock = DockStyle.Bottom };
                Button fontButton = new Button { Text = "Выбрать Шрифт", Dock = DockStyle.Top };

                fontButton.Click += (s, ev) =>
                {
                    FontDialog fontDialog = new FontDialog();
                    if (fontDialog.ShowDialog() == DialogResult.OK)
                    {
                        choosedFont = fontDialog.Font;
                        this.Invalidate();
                    }
                };

                okButton.Click += (s, ev) =>
                {
                    string text = textBox.Text;
                    if (!string.IsNullOrEmpty(text))
                    {
                        TextShape textShape = new TextShape
                        {
                            Text = text,
                            Position = lastPoint.Equals(new Point(0,0)) ? new Point(canvas.Width / 2, canvas.Height / 2) : lastPoint,
                            Color = color1,
                            Font = choosedFont
                        };
                        brushShape = "Select";
                        shapes.Add(textShape);
                        this.Invalidate();
                        selectedShape = textShape;
                    }
                    textForm.Close();
                };

                textForm.Controls.Add(textBox);
                textForm.Controls.Add(okButton);
                textForm.Controls.Add(fontButton);
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

            public virtual void Draw(Graphics g)
            {
                using (Pen pen = new Pen(Color, LineWidth))
                {
                    if (ShapeType == "Линия")
                    {
                        g.DrawLine(pen, StartPoint, EndPoint);
                    }
                    else if (ShapeType == "Прямоугольник")
                    {
                        g.FillRectangle(new SolidBrush(FillColor), Math.Min(StartPoint.X, EndPoint.X), Math.Min(StartPoint.Y, EndPoint.Y),
                            Math.Abs(StartPoint.X - EndPoint.X), Math.Abs(StartPoint.Y - EndPoint.Y));
                        g.DrawRectangle(pen, Math.Min(StartPoint.X, EndPoint.X), Math.Min(StartPoint.Y, EndPoint.Y),
                            Math.Abs(StartPoint.X - EndPoint.X), Math.Abs(StartPoint.Y - EndPoint.Y));
                    }
                    else if (ShapeType == "Эллипс")
                    {
                        g.FillEllipse(new SolidBrush(FillColor), Math.Min(StartPoint.X, EndPoint.X), Math.Min(StartPoint.Y, EndPoint.Y),
                            Math.Abs(StartPoint.X - EndPoint.X), Math.Abs(StartPoint.Y - EndPoint.Y));
                        g.DrawEllipse(pen, Math.Min(StartPoint.X, EndPoint.X), Math.Min(StartPoint.Y, EndPoint.Y),
                            Math.Abs(StartPoint.X - EndPoint.X), Math.Abs(StartPoint.Y - EndPoint.Y));
                    }
                    else if (ShapeType == "Ромб")
                    {
                        Point[] diamondPoints = new Point[]
                        {
                            new Point((StartPoint.X + EndPoint.X) / 2, StartPoint.Y), // Верхняя точка
                            new Point(EndPoint.X, (StartPoint.Y + EndPoint.Y) / 2), // Правая точка
                            new Point((StartPoint.X + EndPoint.X) / 2, EndPoint.Y), // Нижняя точка
                            new Point(StartPoint.X, (StartPoint.Y + EndPoint.Y) / 2) // Левая точка
                        };
                        g.FillPolygon(new SolidBrush(FillColor), diamondPoints);
                        g.DrawPolygon(pen, diamondPoints);
                    }
                    else if (ShapeType == "Треугольник")
                    {
                        Point[] trianglePoints = new Point[]
                        {
                            StartPoint, // Левый угол
                            EndPoint, // Правый угол
                            new Point((StartPoint.X + EndPoint.X) / 2, StartPoint.Y - Math.Abs(StartPoint.Y - EndPoint.Y)) // Верхний угол
                        };
                        g.FillPolygon(new SolidBrush(FillColor), trianglePoints);
                        g.DrawPolygon(pen, trianglePoints);
                    }
                    else if (ShapeType == "Шестиугольник")
                    {
                        Point[] hexagonPoints = new Point[6];
                        for (int i = 0; i < 6; i++)
                        {
                            double angle = Math.PI / 3 * i; // 60 градусов
                            hexagonPoints[i] = new Point(
                                (int)(StartPoint.X + (EndPoint.X - StartPoint.X) / 2 + (EndPoint.X - StartPoint.X) / 2 * Math.Cos(angle)),
                                (int)(StartPoint.Y + (EndPoint.Y - StartPoint.Y) / 2 + (EndPoint.Y - StartPoint.Y) / 2 * Math.Sin(angle))
                            );
                        }
                        g.FillPolygon(new SolidBrush(FillColor), hexagonPoints);
                        g.DrawPolygon(pen, hexagonPoints);
                    }
                }
            }

            public virtual bool Contains(Point p)
            {
                return new Rectangle(Math.Min(StartPoint.X, EndPoint.X), Math.Min(StartPoint.Y, EndPoint.Y),
                            Math.Abs(StartPoint.X - EndPoint.X), Math.Abs(StartPoint.Y - EndPoint.Y)).Contains(p);
            }
        }

        public class TextShape : Shape
        {
            public Point Position { get; set; }
            public string Text { get; set; }
            public Font Font { get; set; }
            public Color Color { get; set; }

            public override void Draw(Graphics g)
            {
                using (Brush brush = new SolidBrush(Color))
                {
                    g.DrawString(Text, Font, brush, Position);
                }
            }

            public Rectangle GetBoundingRectangle()
            {
                using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
                {
                    SizeF size = g.MeasureString(Text, Font);
                    return new Rectangle(Position, size.ToSize());
                }
            }

            public override bool Contains(Point p)
            {
                return GetBoundingRectangle().Contains(p);
            }
        }
    }
}
