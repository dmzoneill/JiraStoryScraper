   M S B A M L   `   `   `  ���� J  FJiraStoryReport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:JiraStoryReportJiraStoryReport       JiraStoryReport.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     /localclr-namespace:JiraStoryReport       � Title$  Jira Story Fetcher��$	��700��$
��1300��.���  � ColumnDefinitions �� $	��350 ��� $	��20* ��� $
��521* � � RowDefinitions �� $��50 ��� $��50 ��� $
��265* ��� $��50 �.+��� -   $��DoWorkButton��$��Fetch JIRA Stories��$��Stretch=�$��10q�$��Stretch=�$��3���� $��Groups��$
��Left=�$��10,1,0,0q�$	��Top=�$	��173��$��26��$��2��$
��Bold"��� -   $��GroupTextBox��$��Stretch=�$��
10,27,10,0q�$	��Top=�$��2��$	��110�� � 	Resources  �� 
TargetType �� Property �� Value��%3   'J�       �� " J�.���  �@��Margin$ 0q�. �/� .T�J�  W� CultureSource$
 User=�.*�.*��� ���	#FFA31515$��12.6666666666667(�$��Consolas)�$0�*DCG DSG SDSG SW Cloud Native Orchestration��(  "System.Windows.Controls.WebBrowser  -   $��
WebBrowser��$��2��$��10,10,10,14q�$��Stretch=�$��Stretch=�$��3���� -   $��IDSIDTextBox��$��Stretch=�$��10,168,10,0q�$	��Top=�$��2��$	��110�� ��%3   'J�       �� " J�.���  �@��Margin$ 0q�. �/� .T�J� $
 User=�.*�.*��� ���	#FFA31515$��12.6666666666667(�$��Consolas)�$�dmoneil2�� �� ���	#FFA31515$��12.6666666666667(�$��Consolas)�J� $
 User=�.*�.*��� ���	#FFA31515$��12.6666666666667(�$��Consolas)�$�plalx��J� $
 User=�.*�.*��� ���	#FFA31515$��12.6666666666667(�$��Consolas)�$�pmossakx���� -   $��NameTextBox��$��Stretch=�$��10,309,10,14q�$��Stretch=�$��2�� ��%3   'J�       �� " J�.���  �@��Margin$ 0q�. �/� .T�J� $
 User=�.*�.*��� ���	#FFA31515$��12.6666666666667(�$��Consolas)�$�Cahill, Francis���� $��IDSID��$
��Left=�$��
10,142,0,0q�$	��Top=�$	��173��$��26��$��2��$
��Bold"��� $��Names��$
��Left=�$��
10,278,0,0q�$	��Top=�$	��173��$��26��$��2��$
��Bold"��� $��Password��$
��Left=�$��
10,10,10,0q�$	��Top=�$��79��$��26��$��1��$
��Bold"��� -   $��UsernameTextBox��$��Stretch=�$��23��$��
94,10,10,0q� �� TextWrapping$
 Wrap=�$��TextBox��$	��Top=��� $��Username��$
��Left=�$��
10,10,10,0q�$	��Top=�$��79��$��26��$
��Bold"�G� -   $��PasswordTextBox��$��Stretch=�$��
94,10,10,0q�$��1��$	��Top=�$��26���� $��1��$��Stretch=�$��5q�$��0��$��4��$��Stretch=�$��5���� -   $��	StatusBar��$�� ��$��2��$��Stretch=�$��10q�$��3��$��Stretch=�