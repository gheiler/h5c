   M S B A M L   `   `   `  ����  HTML5Compiler    HTML5Compiler.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  
MainWindow��5   	   $	��428��6   $	��600��6)    � TaskbarItemInfo 5      *  $System.Windows.Shell.TaskbarItemInfo  5   
      ThumbButtonInfos 5      *  $System.Windows.Shell.ThumbButtonInfo  -   5           ImageSource$  /Resources/Images/play.png��5   "      Description$ Button 1��5	      65   5      .��5      � $	��553��6   .+�5   
   �� -   5        $��image1��5   K   $	��150��6   $
��Left=�6   $��	12,12,0,0q�68   $��Uniform=�6Y   $	��Top=�6k   $	��142��6�   $!��/Resources/Images/html5.png��6�   �� 5   
   -   5        $��
textBlock1��5   O   $��52��6   $
��Left=�6    $��
176,12,0,0q�6;   $����Seleccione la raiz del directorio a compilar, luego escriba el nombre de la carpeta que contiene los modulos y presione en el boton compilar��6a   $	��Top=�6�   $	��315��6  $��WrapWithOverflow=�6  �� 5   
   -   5        $��label1��5   ]   $��Origen:��6   $��28��6"   $
��Left=�6.   $��
176,81,0,0q�6I   $	��Top=�6k   $��60��6�   �� 5   
   -   5        $��	txtOrigin��5   M   $��23��6   $
��Left=�6   $��
242,81,0,0q�69   $	��Top=�6^   $	��237��6v   6�   �� 5   
   -   5        $��label3��5   _   $��Modulos:��6   $��28��6#   $
��Left=�6/   $��176,110,0,0q�6J   $	��Top=�6m   $��60��6�   �� 5   
   -   5        $��txtModulesFolder��5   N   $��23��6   $
��Left=�6   $��242,110,0,0q�69   $��modules��6f   $	��Top=�6u   $	��132��6�   �� 5   
   -   5        $��label2��5   S   $��Destino:��6   $
��Left=�6#   $��176,140,0,0q�6>   $��60��6a   $��28��6l   $	��Top=�6x   �� 5   
   -	   5        $��
txtDestiny��5   N   $��23��6   $
��Left=�6   $��242,139,0,0q�69   $	��Top=�6`   $	��237��6x   6�   �� 5   
   -
   5        $��
btnCompile��5   `   $��Compilar��6   $��23��6$   $
��Left=�60   $��242,259,0,0q�6K   $	��Top=�6r   $��75��6�   6�   � 5   
   -   5        $��compilerProgressBar��5   Q   $��27��6   $
��Left=�6"   $��
90,288,0,0q�6=   $	��Top=�6l   $	��368��6�   �� 5   
   -   5        $��	txtResult��5   P   $��47��6   $
��Left=�6    $��129,330,0,0q�6;   $�� ��6a   $	��Top=�6i   $	��257��6�   $��WrapWithOverflow=�6�   �� 5   
   -   5        $��chkResources��5   i   $��Copiar Recursos��6   $��16��6-   $
��Left=�69   $��242,178,0,0q�6T   $	��Top=�6}   $
��Left=�6�    d� 	IsChecked$ FalseY�6�   {�. 6�   �� 5   
   -   5        $��	chkMinify��5   �   $��Minificar Recursos��6   $��16��60   $
��Left=�6<   $
��Left=�6W   $ FalseY�6y   $��242,200,0,0q�6�   $	��Top=�6�   �� 5   
   -   5        $��btnSaveProject��5   i   $��Guardar Proyecto��6   $��23��6-   $
��Left=�69   $��376,218,0,0q�6T   $	��Top=�6   $	��103��6�   ���	#FFFF6300
����������	#FFFF79276�   5      5      