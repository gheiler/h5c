using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HTML5Compiler.Helpers;
using Ookii.Dialogs.Wpf;
using System.IO;
using System.Windows.Media.Animation;
using System.Text.RegularExpressions;
using Yahoo.Yui.Compressor;
using Dean.Edwards;

namespace HTML5Compiler
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Public Properties

        public string defaultFileName = "default.html";
        public const string StartTag = "<#";
        public const string EndTag = "#>";
        public string Log = string.Empty;
        public DeviceConfig.Devices SelectedDevice = DeviceConfig.Devices.html5;
        public Project OpenProject = new Project();        
        public enum CompilerProperty
        {
            IsSection,
            Type,
            Position,
            FileName,
            OverwriteParent,
            TagName,
            Vars,
            Device
        }
        //public class Instruction: Dictionary<CompilerProperty, string> {}

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("es-AR");
            
            // Asocio tipo de archivo
            if (!FileAssociation.IsAssociated(".h5c"))
            {
                FileAssociation.Associate(".h5c", "ghApps.HTML5Compiler", "HTML5 Compiler Proyect", "H5Clogo-256.ico", AppDomain.CurrentDomain.DomainManager.EntryAssembly.Location);
            }
            // Chequeo si abrio un proyecto desde un archivo
            // Environment.GetCommandLineArgs()
            //if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments != null)
            //{
                string[] arguments = Environment.GetCommandLineArgs();

                if (arguments.Length > 1)
                {
                    // Cargar poryecto
                    try
                    {
                        string filePath = arguments[1]; // string.Empty;
                        //filePath = @"C:\Proyectos\Mobile\gecommerce\trunk\src\MobileViews\mobileView.h5c";                
                        string JSONProject = File.ReadAllText(filePath, Encoding.Default);

                        var oProject = JSONHelper.Deserialize<Project>(JSONProject);
                        // Chequear si los paths de oProject son relativos y modificarlos el path pasaría a estar en "filePath" 

                        if (oProject != null && oProject.Configs != null && oProject.Configs.Count > 0)
                        {
                            LoadProject(oProject);
                            if (arguments.Length > 2)
                            {
                                // TODO: leer el comando y ejecutar (probablemente el unico comando sea "compilar" asi que ewjecutar compilar)
                            }
                        }
                        else {
                            SetDevice(DeviceConfig.Devices.html5);
                            SaveDevice(SelectedDevice);
                        }
                    }
                    catch (Exception ex)
                    {
                        SetDevice(DeviceConfig.Devices.html5);
                        SaveDevice(SelectedDevice);
                        txtResult.Text = "Se produjo un error al cargar el proyecto, recuerde escapar caracteres como \"\\\", etc. El log indica: " + ex.Message + Environment.NewLine;
                    }
                }
                else
                {
                    SetDevice(DeviceConfig.Devices.html5);
                    SaveDevice(SelectedDevice);
                }
            //}
                
        }

        #region Event Handlers

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // DragMove handles all the window placement automatically!
            this.DragMove();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /*private void myWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var m_hWnd = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(m_hWnd).AddHook(WindowProc);
        }

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_SYSCOMMAND)
            {
                if (wParam.ToInt32() == NativeMethods.SC_MINIMIZE)
                {
                    WindowStyle = WindowStyle.SingleBorderWindow;
                    WindowState = WindowState.Minimized;
                    handled = true;
                }
                else if (wParam.ToInt32() == NativeMethods.SC_RESTORE)
                {
                    WindowState = WindowState.Normal;
                    WindowStyle = WindowStyle.None;
                    handled = true;
                }
            }

            return IntPtr.Zero;
        }

        private void minButton_Click(object sender, RoutedEventArgs e)
        {
            NativeMethods.SendMessage(new HandleRef(this, m_hWnd), NativeMethods.WM_SYSCOMMAND, new IntPtr(NativeMethods.SC_MINIMIZE), IntPtr.Zero);
        }*/

        private void btnClose_Click(object sender, EventArgs e)
        {
            mainWindow.Close();
        }

        private void ThumbButtonInfo_Click(object sender, EventArgs e)
        {
            // Thumb Windows 7 para compilar con el boton de play
            txtResult.Text = string.Empty;
            DeviceConfig sConfig = SaveDevice(SelectedDevice);
            foreach (DeviceConfig config in OpenProject.Configs)
            {
                //LoadDevice(config);
                Compile(config);
            }
            //LoadDevice(sConfig); // reload the Device where i was when at start
        } 

        private void txtFolder_GotFocus(object sender, RoutedEventArgs e)
        {
            if (VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                VistaFolderBrowserDialog folderBrowser = new VistaFolderBrowserDialog();
                folderBrowser.ShowDialog();
                txtOrigin.Text = folderBrowser.SelectedPath;
            }
        }

        private void txtDestiny_GotFocus(object sender, RoutedEventArgs e)
        {
            if (VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                VistaFolderBrowserDialog folderBrowser = new VistaFolderBrowserDialog();
                folderBrowser.ShowDialog();
                txtDestiny.Text = folderBrowser.SelectedPath;
            }
        }

        private void btnAddProject_Click(object sender, EventArgs e)
        {
            // TODO: Implement
        }

        private void btnChangeOS_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            SaveDevice(SelectedDevice);
            SetDevice((DeviceConfig.Devices)Enum.Parse(typeof(DeviceConfig.Devices), btn.Name, true));
        }

        private void btnSaveProject_Click(object sender, RoutedEventArgs e)
        {
            if (VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                VistaFolderBrowserDialog oFile = new VistaFolderBrowserDialog();
                oFile.ShowDialog();

                string projectName = txtProjectName.Text;
                if (projectName == string.Empty)
                {
                    projectName = oFile.SelectedPath.Split('\\').Last();                    
                }
                OpenProject.Name = projectName;
                string projectFile = oFile.SelectedPath + "\\" + projectName + ".h5c";

                SaveProject(projectFile);
            }
        }

        private void btnCompile_Click(object sender, RoutedEventArgs e)
        {
            txtResult.Text = string.Empty;
            DeviceConfig sConfig = SaveDevice(SelectedDevice);
            foreach (DeviceConfig config in OpenProject.Configs)
            {
                //LoadDevice(config);
                Compile(config);
            }
            //LoadDevice(sConfig); // reload the Device where i was when at start
        }

        #endregion

        #region Project Managment

        private DeviceConfig SaveDevice(DeviceConfig.Devices selectedDevice)
        {
            DeviceConfig config = GetDeviceFromInputs(selectedDevice);
            if (OpenProject.Configs != null && OpenProject.Configs.Count > 0)
            {
                bool existsDevice = false;
                for (int i = 0; i < OpenProject.Configs.Count; i++)
                {
                    if (OpenProject.Configs[i].Device == selectedDevice) // if exists, replace it
                    {
                        existsDevice = true;
                        OpenProject.Configs[i] = config;
                    }
                }
                if (!existsDevice) // if not in here, add it
                {
                    OpenProject.Configs.Add(config);
                }
            }
            else
            { // if not devices yet, create list and add it
                OpenProject.Configs = new List<DeviceConfig> {config};
            }
            return config;
        }

        private DeviceConfig SetDevice(DeviceConfig.Devices device)
        {
            //visual
            html5.Opacity = 0.3;
            iOS.Opacity = 0.3;
            Android.Opacity = 0.3;
            WindowsPhone.Opacity = 0.3;
            bb.Opacity = 0.3;
            switch (Enum.GetName(typeof(DeviceConfig.Devices), device))
            {
                case "html5":
                    html5.Opacity = 1;
                    break;
                case "iOS":
                    iOS.Opacity = 1;
                    break;
                case "Android":
                    Android.Opacity = 1;
                    break;
                case "WindowsPhone":
                    WindowsPhone.Opacity = 1;
                    break;
                case "bb":
                    bb.Opacity = 1;
                    break;
                default:
                    html5.Opacity = 1;
                    break;
            }
            // functional
            SelectedDevice = device;

            DeviceConfig config = new DeviceConfig();
            if (OpenProject.Configs != null && OpenProject.Configs.Count > 0)
            {
                bool existsDevice = false;
                for (int i = 0; i < OpenProject.Configs.Count; i++)
                {
                    if (OpenProject.Configs[i].Device == SelectedDevice) // if exists, select it
                    {
                        existsDevice = true;
                        config = OpenProject.Configs[i];
                    }
                }
                if (!existsDevice) // if not in here, add it
                {
                    config = SaveDevice(SelectedDevice);
                }
            }
            else
            {
                config = SaveDevice(SelectedDevice);
            }
            LoadDevice(config);
            return config;
        }

        private void LoadDevice(DeviceConfig config)
        {
            txtModulesFolder.Text = config.ModulesFolder;
            txtOrigin.Text = config.Origin;
            txtDestiny.Text = config.Destiny;
            txtBodyDefault.Text = config.DefaultBodyFileName;
            txtDefaultContainer.Text = config.DefaultContainerFileName;
            txtDefaultFooter.Text = config.DefaultFooterFileName;
            txtDefaultHeader.Text = config.DefaultHeaderFileName;
            txtResourcesFolders.Text = config.ResourcesFolders;
            chkMinify.IsChecked = config.MinifyCss;
            chkResources.IsChecked = config.CopyResources;
        }

        private void SaveProject(string filePath)
        {
            SaveDevice(SelectedDevice);
            FileManagment.CreateFileFromString(JSONHelper.Serialize<Project>(OpenProject), filePath);
        }

        private void LoadProject(Project oProject)
        {
            OpenProject = oProject;
            txtProjectName.Text = OpenProject.Name;
            txtModulesFolder.Text = oProject.Configs[0].ModulesFolder;
            txtOrigin.Text = oProject.Configs[0].Origin;
            txtDestiny.Text = oProject.Configs[0].Destiny;
            txtBodyDefault.Text = oProject.Configs[0].DefaultBodyFileName;
            txtDefaultContainer.Text = oProject.Configs[0].DefaultContainerFileName;
            txtDefaultFooter.Text = oProject.Configs[0].DefaultFooterFileName;
            txtDefaultHeader.Text = oProject.Configs[0].DefaultHeaderFileName;
            txtResourcesFolders.Text = oProject.Configs[0].ResourcesFolders;
            chkMinify.IsChecked = oProject.Configs[0].MinifyCss;
            chkResources.IsChecked = oProject.Configs[0].CopyResources;

            SetDevice(oProject.Configs[0].Device);
        }        

        private DeviceConfig GetDeviceFromInputs(DeviceConfig.Devices device)
        {
            DeviceConfig config = new DeviceConfig();
            config.ModulesFolder = txtModulesFolder.Text;
            config.Origin = txtOrigin.Text;
            config.Destiny = txtDestiny.Text;
            config.DefaultBodyFileName = txtBodyDefault.Text;
            config.DefaultContainerFileName = txtDefaultContainer.Text;
            config.DefaultFooterFileName = txtDefaultFooter.Text;
            config.DefaultHeaderFileName = txtDefaultHeader.Text;
            config.ResourcesFolders = txtResourcesFolders.Text;
            config.MinifyCss = (bool)chkMinify.IsChecked;
            config.CopyResources = (bool)chkResources.IsChecked;
            config.Device = device;
            return config;
        }

        #endregion

        private void Compile(DeviceConfig config)
        {
            MoveProgressBar(0);            
            string deviceName = Enum.GetName(typeof(DeviceConfig.Devices), config.Device);
            string proyectFolder = config.Origin;
            string destinyFolder = config.Destiny;
            string modulesFolder = System.IO.Path.Combine(proyectFolder, config.ModulesFolder);
            string defaultContainerFilePath = System.IO.Path.Combine(modulesFolder, config.DefaultContainerFileName);
            string defaultHeaderFilePath = System.IO.Path.Combine(modulesFolder, "header", config.DefaultHeaderFileName);
            string defaultFooterFilePath = System.IO.Path.Combine(modulesFolder, "footer", config.DefaultFooterFileName);

            if (Directory.Exists(proyectFolder)) // existe el proyecto
            {
                if (Directory.Exists(destinyFolder)) // existe la carpeta destino
                {
                    if (Directory.Exists(modulesFolder)) // existe la carpeta con los modulos
                    {
                        // chequeo que exista el archivo base en donde se compilaran los modulos
                        if (!File.Exists(defaultContainerFilePath))
                        {
                            txtResult.Text += String.Format("{0} - No existe el archivo: {1} en el directorio raiz del proyecto: {2} {3}", deviceName, defaultFileName, proyectFolder, Environment.NewLine);
                            return;
                        }

                        string defaultContainerFileContent = string.Empty;
                        string defaultHeaderFileContent = string.Empty;
                        string defaultFooterFileContent = string.Empty;

                        string defaultCompleteContent = defaultContainerFileContent = System.IO.File.ReadAllText(defaultContainerFilePath, Encoding.UTF8);

                        if (File.Exists(defaultHeaderFilePath))
                        {
                            defaultHeaderFileContent = System.IO.File.ReadAllText(defaultHeaderFilePath, Encoding.UTF8);
                            defaultCompleteContent = ReplaceInstructionPlace(defaultCompleteContent, "header", defaultHeaderFileContent);
                        }
                        if (File.Exists(defaultFooterFilePath))
                        {
                            defaultFooterFileContent = System.IO.File.ReadAllText(defaultFooterFilePath, Encoding.UTF8);
                            defaultCompleteContent = ReplaceInstructionPlace(defaultCompleteContent, "footer", defaultFooterFileContent);
                        }

                        // Obtengo la lista de archivos a ser compilados
                        List<string> filePaths = Directory.GetFiles(System.IO.Path.Combine(modulesFolder, "body")).ToList();

                        // Genero Htmls
                        double percentage = (1.0 / Convert.ToDouble(filePaths.Count)) * 50; // Supongo el compilado de archivos el 50% del total
                        foreach (string filePath in filePaths) // recorro los archivos del body
                        {
                            string headerContent = defaultHeaderFileContent; // contenido default del header
                            string footerContent = defaultFooterFileContent; // contenido default del footer
                            string cointanerContent = defaultContainerFileContent;
                            string completeContent = defaultCompleteContent; 

                            string bodyFileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                            string bodyContent = System.IO.File.ReadAllText(filePath, Encoding.UTF8); // seteo el contenido del body

                            List<Dictionary<CompilerProperty, string>> bodyInstructions = GetInstructions(bodyContent);

                            if (bodyInstructions.Count > 0)
                            {
                                foreach (Dictionary<CompilerProperty, string> Instruction in bodyInstructions) // Recorro las instrucciones del body
                                {
                                    string position, fileName, vars, device;
                                    string contentBeenModified = string.Empty;
                                    List<string> kvVars;
                                    Instruction.TryGetValue(CompilerProperty.Position, out position);
                                    Instruction.TryGetValue(CompilerProperty.FileName, out fileName);
                                    Instruction.TryGetValue(CompilerProperty.Vars, out vars);
                                    Instruction.TryGetValue(CompilerProperty.Device, out device);

                                    if (position != null)
                                    {
                                        if (device != null && config.Device != (DeviceConfig.Devices)Enum.Parse(typeof(DeviceConfig.Devices), device, true))
                                        {
                                            continue; // si la instruccion es especifica de algun dispositivo, solo aplico a ese
                                        }

                                        if (fileName != null)
                                        {
                                            try
                                            {
                                                contentBeenModified = System.IO.File.ReadAllText(System.IO.Path.Combine(proyectFolder, fileName), Encoding.UTF8);
                                            }
                                            catch
                                            {
                                                txtResult.Text += string.Format("{0} - El nombre del archivo {1} esta mal formateado o no existe {2}", deviceName, fileName, Environment.NewLine);
                                            }
                                        }
                                        
                                        if (fileName == null && vars != null) // Si solo planeo cargar vars en algun archivo default
                                        {
                                            switch (position) // asigno el archivo default corresdpondiente a la posicion al contenido que esta siendo modificado
                                            {
                                                case "header":
                                                    contentBeenModified = headerContent;
                                                    break;
                                                case "body":
                                                    contentBeenModified = cointanerContent;
                                                    break;
                                                case "footer":
                                                    contentBeenModified = footerContent;
                                                    break;
                                            }
                                        }

                                        if (vars != null) // Busco si hay variables a remplazar en el html parcial a cargar
                                        {
                                            kvVars = vars.Split(',').ToList(); // TODO: cambiar "," por variable
                                            foreach (var kvVar in kvVars)
                                            {
                                                if (kvVar.Split(':').Length > 1) // si no tiene valor no remplazo // TODO: cambiar ":" por variable
                                                {
                                                    contentBeenModified = ReplaceInstructionVar(kvVar.Split(':')[0].Trim(), kvVar.Split(':')[1].Trim(), contentBeenModified);
                                                }
                                            }
                                        }

                                        // TODO: Aca si correspondiera deberia de forma recursiva cargar mas html para el contenido siendo modificado                                        

                                        switch (position) // asigno el contenido modificado a la seccion correspondiente
                                        {
                                            case "header":
                                                headerContent = contentBeenModified;
                                                break;
                                            case "body":
                                                cointanerContent = contentBeenModified;
                                                break;
                                            case "footer":
                                                footerContent = contentBeenModified;
                                                break;
                                        }
                                    }

                                }
                                completeContent = ReplaceInstructionPlace(cointanerContent, "header", headerContent);
                                completeContent = ReplaceInstructionPlace(completeContent, "footer", footerContent);

                            }

                            string moduleContent = ReplaceInstructionPlace(completeContent, "body", bodyContent); // index con header, body y footer ya remplazado
                            moduleContent = RemoveInstructions(moduleContent); // remuevo las instrucciones que puedan haber quedado
                            
                            string destinyFilePath = Path.Combine(destinyFolder, string.Concat(bodyFileName, ".html"));
                            FileManagment.CreateFileFromString(moduleContent, destinyFilePath);

                            // Move status bar
                            MoveProgressBar(percentage);
                        }

                        // Copy Resources
                        if ((bool)config.CopyResources)
                        {
                            List<string> lResourcesFolders = config.ResourcesFolders.Split(',').ToList();
                            //TODO: copiar carpetas especificadas y no las que a mi se me cantó
                            string resFileName = string.Empty;
                            string resDestName = string.Empty;
                            int i = 1;
                            foreach (string rFolder in lResourcesFolders)
                            {
                                var sFolder = rFolder.Trim();
                                bool minify = false;
                                // TODO: agregar una variable minificar js
                                //if (rFolder == "js") {
                                //    minify = (bool)config.MinifyJs;                                    
                                //} else {
                                //    minify = (bool)config.MinifyCss;
                                //}
                                minify = (bool)config.MinifyCss;
                                string originPath = System.IO.Path.Combine(proyectFolder, sFolder);
                                string destinyPath = System.IO.Path.Combine(destinyFolder, sFolder);
                                FileManagment.DirectoryCopy(originPath, destinyPath, true, minify);
                                MoveProgressBar(50 + (40 / lResourcesFolders.Count * i));
                                i++;
                            }

                            /*
                            // Css Minify
                            string cssOriginPath = System.IO.Path.Combine(proyectFolder, "css");
                            string cssDestinyPath = System.IO.Path.Combine(destinyFolder, "css");
                            DirectoryCopy(cssOriginPath, cssDestinyPath, true, (bool)config.MinifyCss);
                            MoveProgressBar(65);

                            // Js Minify
                            string jsOriginPath = System.IO.Path.Combine(proyectFolder, "js");
                            string jsDestinyPath = System.IO.Path.Combine(destinyFolder, "js");
                            DirectoryCopy(jsOriginPath, jsDestinyPath, true, (bool)config.MinifyCss);
                            MoveProgressBar(80);

                            // Image Reduction
                            string imgOriginPath = System.IO.Path.Combine(proyectFolder, "img");
                            string imgDestinyPath = System.IO.Path.Combine(destinyFolder, "img");
                            DirectoryCopy(imgOriginPath, imgDestinyPath, true, (bool)config.MinifyCss);
                            MoveProgressBar(90);*/
                        }

                        // Show Log

                        // Finish Status Bar
                        txtResult.Text += String.Format("{0} - Compilacion Finalizada Correctamente! {1}", deviceName, Environment.NewLine);
                        MoveProgressBar(100);
                    }
                    else
                    {
                        txtResult.Text += String.Format("{0} - El directorio de los modulos a compilar no existe o no tiene permisos de lectura {1}", deviceName, Environment.NewLine);
                        MoveProgressBar(0);
                    }
                }
                else
                {
                    txtResult.Text += String.Format("{0} - El directorio de destino del proyecto no existe o no tiene permisos de lectura/escritura {1}", deviceName, Environment.NewLine);
                    MoveProgressBar(0);
                }
            }
            else
            {
                txtResult.Text += String.Format("{0} - El directorio del proyecto no existe o no tiene permisos de lectura {1}", deviceName, Environment.NewLine);
                MoveProgressBar(0);
            }
        }

        #region Compilation Instructions

        private string ReplaceInstructionVar(string key, string value, string content)
        {
            // busco todos las variables <# key #>
            string sVar = string.Concat(StartTag, "[ \n]*?", key, "[ \n]*?", EndTag);
            MatchCollection oMatches = Regex.Matches(content, sVar);
            foreach (System.Text.RegularExpressions.Match oMatch in oMatches)
            {
                content = content.Replace(oMatch.ToString(), value); // remplazo por el valor
            }
            return content;
        }

        private string RemoveInstructions(string content)
        {
            // busco todos los tags de instrucciones <#(.|\n)*?#>
            MatchCollection oMatchesInstructions = Regex.Matches(content, string.Concat(StartTag,"(.|\n)*?", EndTag));
            foreach (System.Text.RegularExpressions.Match oMatch in oMatchesInstructions)
            {
                content = content.Replace(oMatch.ToString(), string.Empty);// remplazo por un espacio vacio
            }

            // busco todos los comentarios de html <!-- -->
            MatchCollection oMatchesComments = Regex.Matches(content, string.Concat("<!--", "(.|\n)*?", "-->"));
            foreach (System.Text.RegularExpressions.Match oMatch in oMatchesComments)
            {
                content = content.Replace(oMatch.ToString(), string.Empty);// remplazo por un espacio vacio
            }

            // busco todos los espacios en blanco
            /* MatchCollection oMatchesWhiteSpaces = Regex.Matches(content, "\r\n");
            foreach (System.Text.RegularExpressions.Match oMatch in oMatchesWhiteSpaces)
            {
                content = content.Replace(oMatch.ToString(), string.Empty);// remplazo por un espacio vacio cada instruccion
            }*/
            return content;
        }

        private void MoveProgressBar(double percentage)
        {            
            Dispatcher.Invoke(new Action(() => {
                // this will happen in a separate thread
                Duration duration = new Duration(TimeSpan.FromSeconds(1));
                DoubleAnimation doubleanimation = new DoubleAnimation(percentage, duration);
                compilerProgressBar.BeginAnimation(ProgressBar.ValueProperty, doubleanimation);
                compilerProgressBar.UpdateLayout();
                compilerProgressBar.Value = percentage;
            }), System.Windows.Threading.DispatcherPriority.ContextIdle, null);
        }

        private List<string> GetFilePlaces(string filePath)
        {
            List<string> Places = new List<string>();
            using (var reader = new StreamReader(filePath))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    line = reader.ReadLine();
                    if (IsInstructionPlace(line))
                    {
                         // TODO: Creo que no es necesario este metodo
                    }
                }
            }
            return Places;
        }

        private bool IsSection(string filePath)
        {
            bool isSection = false;
            using (var reader = new StreamReader(filePath))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    line = reader.ReadLine();
                    if (IsInstruction(line))
                    {
                        Dictionary<CompilerProperty, string> propertiesValues = GetCompilerPropertiesValues(line);
                        foreach (KeyValuePair<CompilerProperty, String> property in propertiesValues)
                        {
                            if (property.Key == CompilerProperty.IsSection)
                            {
                                if (property.Value != "false") { isSection = true; }
                            }
                        }
                    }
                    else
                    { break; }
                }
            }
            return isSection;
        }

        private bool IsInstruction(string line)
        {
            return line.Trim().Length > (StartTag.Length + EndTag.Length) && // la linea tiene mas caracteres que la suma de los tags
                   line.Trim().Substring(0, StartTag.Length) == StartTag && // Existe el tag de inicio
                   line.Trim().Substring(line.Trim().Length - EndTag.Length, EndTag.Length) == EndTag; // Existe el tag final
        }
        
        private bool IsInstructionPlace(string line)
        {
            return line.Trim().Length > ("<!--".Length + StartTag.Length + EndTag.Length + "-->".Length) && // la linea tiene mas caracteres que la suma de los tags
                   line.Trim().Substring(0, "<!--".Length + StartTag.Length) == string.Concat("<!--", StartTag) && // Existe el tag de inicio
                   line.Trim().Substring(line.Trim().Length - EndTag.Length - "-->".Length, EndTag.Length + "-->".Length) == string.Concat(EndTag, "-->"); // Existe el tag final
        }

        private string ReplaceInstructionPlace(string fileContent, string position, string positionContent)
        {
            //byte[] byteArray = Encoding.UTF8.GetBytes(fileContent);
            byte[] byteArray = new UTF8Encoding(true).GetBytes(fileContent);
            MemoryStream stream = new MemoryStream(byteArray);
            string newContent = string.Empty;

            using (var reader = new StreamReader(stream))
            {
                var line = string.Empty;
                while (line != null)
                {
                    line = reader.ReadLine();
                    if (line != null) // pffff Daaah!
                    {
                        if (IsInstructionPlace(line))
                        {
                            foreach (KeyValuePair<CompilerProperty, string> propertyValue in GetCompilerPropertiesValues(line))
                            {
                                if (propertyValue.Key == CompilerProperty.Position)
                                {
                                    if (propertyValue.Value == position) // si la posicion coincide con la indicada remplazo la instruccion con el modulo a incluir
                                    {
                                        line = positionContent;
                                    }
                                }
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            newContent = string.Concat(newContent, Environment.NewLine, line); // añado la linea al string que devulevo // TODO: quitar la NewLine, remplazar todo esto por un regex?
                        }
                    }
                }
            }
            return newContent;
        }

        private List<Dictionary<CompilerProperty, string>> GetInstructions(string fileContent)
        {
            List<Dictionary<CompilerProperty, string>> Instructions = new List<Dictionary<CompilerProperty, string>>();

            //byte[] byteArray = Encoding.Default.GetBytes(fileContent);
            byte[] byteArray = new UTF8Encoding(true).GetBytes(fileContent);
            MemoryStream stream = new MemoryStream(byteArray);

            using (var reader = new StreamReader(stream))
            {
                var line = string.Empty;
                while (line != null)
                {
                    line = reader.ReadLine();
                    if (line != null) // pffff Daaah!
                    {
                        if (IsInstruction(line))
                        {
                            Dictionary<CompilerProperty, string> propertiesValues = GetCompilerPropertiesValues(line);
                            Instructions.Add(propertiesValues);
                        }
                    }
                }
            }
            return Instructions;
        }

        private Dictionary<CompilerProperty, string> GetCompilerPropertiesValues(string line)
        {
            Dictionary<CompilerProperty, string> propertiesValues = new Dictionary<CompilerProperty, string>();
            
            // Formateo la linea sacando informacion no relacionada con las propiedades
            line = line.Trim().Replace("<!--",string.Empty).Replace("-->", string.Empty).Replace(StartTag, string.Empty).Replace(EndTag, string.Empty);

            // Separa todas las propiedades ej: [ {isSection}, {fileName="\modules\header\default.html"}, { type="WrappedBy" } ]
            List<string> propertiesAndValues = line.Split(new string []{ "\" "}, StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (var propertyAndValue in propertiesAndValues)
            { 
                // separo en nombre de la propiedad y valor 
                string sProperty = propertyAndValue.Split('=')[0].Trim();
                string pValue = string.Empty;
                if (!string.IsNullOrWhiteSpace(sProperty))
                {
                    try
                    {
                        // la convierto a una propiedad definida
                        CompilerProperty property = (CompilerProperty)Enum.Parse(typeof(CompilerProperty), sProperty, true);
                        // la agrego a la lista de kv que devulevo si corresponde
                        if (propertyAndValue.Split('=').Length > 1) { pValue = propertyAndValue.Split('=')[1].Trim().Trim('"'); }
                        propertiesValues.Add(property, pValue);
                    }
                    catch { /* TODO: Do Something?? */ }
                }
            }
            return propertiesValues;
        }

        #endregion
        
    }

}