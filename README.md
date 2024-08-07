# 🍽️ Recipe Book Application

A comprehensive Recipe Book application written in C#. This application allows users to save, manage, and share their favorite recipes seamlessly. Below are the key features and functionalities of the application:

![1](https://github.com/user-attachments/assets/7c6bf5c2-6df7-432b-a6e0-55fcb14c5cc9)

![2](https://github.com/user-attachments/assets/17045112-9017-4689-b4de-0769d19f32c7)

![3](https://github.com/user-attachments/assets/daada79d-401a-4f1d-9fa2-c17ec51bff0e)

![4](https://github.com/user-attachments/assets/0362020c-fe51-4d8c-b15b-d74f081067ab)

## ✨ Features

- **Recipe Management**: Users can save their favorite recipes with details such as ingredients, steps, and photos.
- **Email Notifications**: Every new recipe addition triggers an email notification to all registered users using the Outlook API.
- **User Authentication**: The system supports user registration and login with a username and password.
- **Favorites**: Users can mark recipes as favorites and view them separately.
- **Permissions System**: Only administrators have the authority to add new recipes.
- **Recipe Operations**: Users can add, edit, and delete recipes.
- **Search Functionality**: Users can search for recipes globally or within their favorites.
- **Animations and Loader**: The login screen features an animated loader for a smooth user experience.
- **Drag and Drop**: Despite the flat design, the form window can be dragged and dropped using the mouse.

## 📚 Technical Details

- **Database**: The application uses an Access database to store user and recipe information.
- **Remember Me Feature**: The login screen includes a "Remember Me" option that saves the user's email and password in the computer's registry for quick access.
- **Image Handling**: Images are saved and retrieved as binary data (2-byte) in the database.

## 💼 Requirements

- **.NET Framework**: Ensure you have the latest version of the .NET Framework installed.
- **Microsoft Outlook**: The Outlook API is used for sending email notifications.

## 🔧 Installation

1. **Clone the Repository**:
    ```bash
    git clone https://github.com/saliholoji/Recipe-Book-Application.git
    ```
2. **Open in Visual Studio**:
    Open the solution file `yemekUygulamasi.sln` in Visual Studio.
3. **Configure Database**:
    Ensure the Access database file is correctly linked and the connection string is set up in the application settings.
4. **Build and Run**:
    Build the project and run the application.

## 👥 Contributing

We welcome contributions! Please fork the repository and submit pull requests for any enhancements or bug fixes.

## 📧 Contact

For any inquiries or support, please contact [info@salihuysal.com](mailto:info@salihuysal.com).

---

Thank you for using the Recipe Book application! Enjoy managing and sharing your favorite recipes with ease. 🍴

---

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE) [![GitHub stars](https://img.shields.io/github/stars/saliholoji/Recipe-Book-Application.svg)](https://github.com/saliholoji/Recipe-Book-Application/stargazers) [![GitHub forks](https://img.shields.io/github/forks/saliholoji/Recipe-Book-Application.svg)](https://github.com/saliholoji/Recipe-Book-Application/network)
