using ChatBusinessTier;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ChatDLL;
using System.ServiceModel.Security;
using System.Windows.Threading;
using System.Threading;
using Microsoft.Win32;
using Path = System.IO.Path;

namespace Client
{
    /// <summary>
    /// Interaction logic for ChatroomScreen.xaml
    /// </summary>
    /// 


    // GUI Components Used: TextBox, Button
    // ListBox & ListBoxItem: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/listbox?view=netframeworkdesktop-4.
    // DispatcherTimer: https://learn.microsoft.com/en-us/dotnet/api/system.windows.threading.dispatchertimer?view=windowsdesktop-7.0
    // DispatcherTimer creates a new thread to run a task based on intervals,this allows us
    // to schedule updates to GUI components with Dispatcher.Invoke() without freezing the GUI

    public partial class ChatroomScreen : Window
    {
        private string currentChatRoom;
        private string username;
        private ChatBusinessInterface server;
        private bool isRunning = true;
        private DispatcherTimer timer;
        private DispatcherTimer fileListUpdater;
        private List<FileMessage> filesList;


        public ChatroomScreen(string username, ChatBusinessInterface server)
        {
            this.username = username;
            this.server = server;
            InitializeComponent();
            Dictionary<string, List<string>> chatrooms = server.GetChatRooms();
            List<string> chatroomNames = chatrooms.Keys.ToList();
            ChatList.SelectionMode = SelectionMode.Single;
            foreach (string chatroomName in chatroomNames)
            {
                ListBoxItem chatroomItem = createChatBoxItem(chatroomName);
                ChatList.Items.Add(chatroomItem);
            }
            ChatMessage.Text = "Click on a chat room to begin!";
            ChatMessage.IsEnabled = false;
            MessageBox.IsEnabled = false;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerTickGUI;
            timer.Start();


            fileListUpdater = new DispatcherTimer();
            fileListUpdater.Interval = TimeSpan.FromSeconds(3);
            fileListUpdater.Tick += TimerTickFilesGUI;
            fileListUpdater.Start();
        }

        private void TimerTickGUI(object sender, EventArgs e)
        {
            updateGUI();
        }


        private void TimerTickFilesGUI(object sender, EventArgs e)
        {
            updateFileList();
        }

        private void OnClickSend(object sender, RoutedEventArgs e)
        {
            string message = MessageBox.Text.Trim();
            if (currentChatRoom != null)
            {
                if (!message.Equals(""))
                {
                    server.SendMessage(username, currentChatRoom, new TextMessage(username, message));
                }
            }
        }

        private void OnPrivateMessageClick(object sender, RoutedEventArgs e)
        {
            string message = MessageBox.Text.Trim();
            if (currentChatRoom != null)
            {
                if (!message.Equals(""))
                {
                    List<string> selectedUsers = new List<string>();
                    foreach(ListBoxItem item in UsersInChat.Items)
                    {
                        CheckBox checkBox = item.Content as CheckBox;
                        if(checkBox != null && checkBox.IsChecked == true)
                        {
                            string receiver = checkBox.Content.ToString();
                            bool success = server.SendPrivateMessage(username, receiver, new TextMessage(username, message));
                            if(!success)
                            {
                                Modal modal = new Modal();
                                modal.setString("Failed to send private message from: " + username + ", receiver: " + receiver);
                                modal.Show();
                            }
                        }
                    }
                }
            }
        }


        private void OnClickViewPrivate(object sender, RoutedEventArgs e)
        {
            List<TextMessage> privateMessages = server.GetUserPrivateMessages(this.username);
            PrivateMessageBox privateMessageBox = new PrivateMessageBox();
            privateMessageBox.setMessages(privateMessages);
            privateMessageBox.Show();
        }

        private void OnRefreshClick(object sender, RoutedEventArgs e)
        {
            updateGUI();
        }

        private void OnLogoutClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            server.Logout(username);
            mainWindow.Show();
            timer.Stop();
            fileListUpdater.Stop();
            Close();
        }

        private void OnClickCreateRoom(object sender, RoutedEventArgs e)
        {
            string newRoom = NewRoomInput.Text.Trim();
            if(newRoom.Equals(""))
            {
                Modal errorModal = new Modal();
                errorModal.setString("New room name is empty.");
            }
            else
            {
                bool success = server.CreateRoom(newRoom);
                if(success)
                {
                    Modal errorModal = new Modal();
                    errorModal.setString("Error creating room.");
                }
                else
                {
                    Modal errorModal = new Modal();
                    errorModal.setString("Error creating room.");
                }
            }
        }

        private void OnWindowClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            server.Logout(username);
            MainWindow loginScreen = new MainWindow();
            loginScreen.Show();
        }


        private void OnClickUploadFile(object sender, RoutedEventArgs e)
        {
            // Don't allow upload if there is no chatroom
            if(currentChatRoom == null)
            {
                return;
            }
            OpenFileDialog fileSelector = new OpenFileDialog();
            fileSelector.Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            fileSelector.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (fileSelector.ShowDialog() == true)
            {
                string selectedFilePath = fileSelector.FileName;

                string messageLog = "\n" + username + " uploaded a file" + selectedFilePath + " to the chatroom.";

                // Allow file size less than 2MB
                long fileSizeInBytes = new FileInfo(selectedFilePath).Length;
                long maxSizeInBytes = 2 * 1024 * 1024; 
                if(fileSizeInBytes > maxSizeInBytes)
                {
                    Modal modal = new Modal();
                    modal.setString("File must be 2MB or less");
                    modal.Show();
                }
                else
                {
                    // Store file data to file server
                    byte[] data = File.ReadAllBytes(selectedFilePath);
                    FileMessage newFile = new FileMessage(username, currentChatRoom,
                                                            Path.GetFileName(selectedFilePath),
                                                            Path.GetExtension(selectedFilePath),
                                                            data);
                    server.UploadFile(currentChatRoom, newFile);
                }

            }
        }

        private void OnClickChatRoom(object sender, RoutedEventArgs e) 
        {
            if (sender is ListBoxItem clickItem)
            {

                // get the clicked chatroom and check if user already joined
                string chatroom = clickItem.Content.ToString();
                bool verified = server.VerifyUserInChat(username, chatroom);
                // User verified in chat room, switch current chatroom and clear the user lists
                if (verified)
                {
                    currentChatRoom = chatroom;
                    UsersInChat.Items.Clear();
                }
                else
                {
                    // Ask user to join chatroom
                    ConfirmModal modal = new ConfirmModal("Do you want to join: " + chatroom + "?");
                    modal.ShowDialog();
                    bool confirmed = modal.confirmed;
                    // Join when user clicks confirm
                    if(confirmed)
                    {
                        server.JoinRoom(username,chatroom);
                        currentChatRoom = chatroom;
                        MessageBox.IsEnabled = true;
                    }
                }
            }
        }


        // User leave room
        public void OnClickLeaveRoom(object sender, RoutedEventArgs e)
        {
            string currentRoom = NewRoomInput.Text.Trim();
            if (currentRoom.Equals(""))
            {
                Modal errorModal = new Modal();
                errorModal.setString("New room name is empty.");
                errorModal.Show();
            }
            else
            {
                bool success = server.LeaveRoom(this.username, currentRoom);
                if (success)
                {
                    Modal successModal = new Modal();
                    successModal.setString("You left: " + currentRoom);
                    successModal.Show();
                }
                else
                {
                    Modal errorModal = new Modal();
                    errorModal.setString("Failed to leave room: " + currentRoom);
                    errorModal.Show();
                }
                if(currentChatRoom.Equals(currentRoom))
                {
                    this.currentChatRoom = null;
                }
            }
        }


        // creates a ListBoxItem for the chat list
        private ListBoxItem createChatBoxItem(string name)
        {
            ListBoxItem chatroomItem = new ListBoxItem
            {
                Content = name,
                IsHitTestVisible = true,
            };
            chatroomItem.PreviewMouseDoubleClick += OnClickChatRoom;
            return chatroomItem;
        }


        // creates a ListBoxList for the users list
        private ListBoxItem createUserBoxItem(string username)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.Content = username;
            checkBox.IsChecked = false;
            ListBoxItem userItem = new ListBoxItem 
            { 
                Content = checkBox, IsHitTestVisible = !this.username.Equals(username)
            };
            return userItem;
        }


        // Creates a ListBoxItem for the files list
        private ListBoxItem createFileBoxItem(string filename)
        {
            ListBoxItem fileItem = new ListBoxItem
            {
                Content = filename,
                IsHitTestVisible = true,
            };
            fileItem.PreviewMouseDoubleClick += OnClickFile;

            return fileItem;
        }


        // Called when a file is clicked, Downloads to the user's selected file path when the save dialog box is opened
        // Reference: https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/how-to-save-files-using-the-savefiledialog-component?view=netframeworkdesktop-4.8 
        private void OnClickFile(object sender, EventArgs e)
        {
            //Save file on User pc
            ListBoxItem selectedFileItem = sender as ListBoxItem;
            int index = SharedFilesList.Items.IndexOf(selectedFileItem);
            FileMessage file = filesList[index];
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = file.getFileName(),
            };

            bool? result = saveFileDialog.ShowDialog();

            if(result == true)
            {
                string filePath = saveFileDialog.FileName;
                try
                {
                    // Save the file data to the selected file path, Use byte array instead of standard input for image data as well
                    // https://www.geeksforgeeks.org/file-readallbytes-method-in-csharp-with-examples/
                    File.WriteAllBytes(filePath, file.getFileData());

                    Modal modal = new Modal();
                    modal.setString("Saved file to path: " +  filePath);
                    modal.Show();
                }
                catch (Exception ex)
                {
                    Modal modal = new Modal();
                    modal.setString("Download failed: " + ex.Message);
                    modal.Show();
                }
            }
        }


        // Updates the list of files when switching chatrooms
        private void updateFileList()
        {
            Console.WriteLine("Updating files list");
            // Do not update files list if user is not in a chatroom
            if(currentChatRoom == null) 
            {
                // resets the list of files in the chatroom
                SharedFilesList.Items.Clear();
                this.filesList = new List<FileMessage>();
                return;
            }
            // Update shared files list with the new list queried from the file server
            SharedFilesList.Dispatcher.Invoke(new Action(() =>
            {
                List<FileMessage> newFiles = server.GetFilesForChatRoom(currentChatRoom);
                this.filesList = newFiles;
                SharedFilesList.Items.Clear();
                foreach (FileMessage file in newFiles) 
                {
                    ListBoxItem updatedItem = createFileBoxItem(file.getFileName());
                    SharedFilesList.Items.Add(updatedItem);
                }
            }));
        }


        // updates GUI components, TextChat Box and RoomC hats List
        private void updateGUI()
        {
            Dispatcher.Invoke(() =>
            { 
                //Fetch updated list of chat room names
                Dictionary<string, List<string>> chatrooms = server.GetChatRooms();
                List<string> chatNames = chatrooms.Keys.ToList();

                // Get the list of current chat rooms in GUI
                Dictionary<string, bool> chatRoomsPresent = new Dictionary<string, bool>();



                List<string> usersToAdd = new List<string>();


                // Check if user is in a chatroom
                if (currentChatRoom != null)
                {
                    
                    Dictionary<string, bool> usersPresent = new Dictionary<string, bool>();
                    List<string> users = server.GetUsersInChat(currentChatRoom);
                    List<CheckBox> itemsToRemove = new List<CheckBox>();
                    foreach(ListBoxItem item in UsersInChat.Items) 
                    {

                        Console.WriteLine(item.Content.ToString());
                        CheckBox checkBox = item.Content as CheckBox;
                        // check if user is in chat
                        if (server.VerifyUserInChat(checkBox.Content.ToString(), currentChatRoom))
                        {
                            usersPresent.Add(checkBox.Content.ToString(), true);
                        }
                        // Save user to be removed later
                        else
                        {
                            itemsToRemove.Add(checkBox);
                        }
                    }

                    // Remove old users from the user list that left.
                    foreach(CheckBox checkBox in itemsToRemove)
                    {
                        UsersInChat.Items.Remove(checkBox);
                    }
                    
                    // Check any new users that joined and add to chat
                    foreach(string user in users)
                    {
                        if(!usersPresent.ContainsKey(user))
                        {
                            usersToAdd.Add(user);
                        }
                    }
                }


                foreach (string chatroom in chatrooms.Keys)
                {
                    Console.WriteLine(chatroom + "=" + chatroom);   
                }

                // Keeps a track of current rooms in the list box
                foreach (ListBoxItem item in ChatList.Items) 
                { 
                    chatRoomsPresent.Add(item.Content.ToString(), true);
                }


                // Set a container for new chats to be added to the list box of chat room names
                List<string> chatRoomsToAdd = new List<string>();

                // cross check the new chat room information for any new chat rooms created.
                // Store the new names in a list
                foreach (string roomName in chatNames)
                {
                    if (!chatRoomsPresent.ContainsKey(roomName))
                    {
                        Console.WriteLine("Added a new chat room: " +  roomName);
                        chatRoomsToAdd.Add(roomName);
                    }
                }

                string updatedMessage = ChatMessage.Text.ToString();
                if (currentChatRoom != null)
                {
                    // Fetch udpated messages of currently viewed chat room
                    List<string> stringNewMessages = new List<string>();
                    List<TextMessage> updatedTextMessages = server.GetMessagesForChatRoom(currentChatRoom);
                    if(updatedTextMessages == null)
                    {
                        updatedMessage = "No messages in this chat yet.";
                    }
                    else
                    {
                        // Join all messages in a single string to update the chat text box
                        foreach (TextMessage message in updatedTextMessages)
                        {
                            stringNewMessages.Add(message.getTimeStamp() + " " + message.getSender() + ": " + message.getMessage());
                        }
                        updatedMessage = string.Join("\n", stringNewMessages);
                    }
                }
                else
                {
                    updatedMessage = "You are currently not in a chatroom. Join a chat room to see messages.";
                    UsersInChat.Items.Clear();
                }


                // Update the chat room list box for new chat rooms created
                foreach (string chatroom in chatRoomsToAdd)
                {
                    ListBoxItem newItem = createChatBoxItem(chatroom);
                    ChatList.Items.Add(newItem);
                }
                // Update current chat room text
                ChatMessage.Text = updatedMessage;


                // Update the User list box for new users joining current room
                foreach(string user in usersToAdd)
                {
                    ListBoxItem newUserBox = createUserBoxItem(user);
                    UsersInChat.Items.Add(newUserBox);
                }
            });

        }

    }
}
