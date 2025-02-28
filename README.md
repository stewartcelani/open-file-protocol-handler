# Windows File Protocol Handler

A lightweight Windows application that registers a custom URL protocol handler (`open://`) to open files and folders directly from web pages or other applications.

## Features

- Register a custom `open://` URL protocol on Windows systems
- Dynamically detect whether a path is a file or folder and open it appropriately
- Open files with their default associated applications
- Open folders in Windows Explorer
- Simple error handling with visual feedback
- Debug mode for easier troubleshooting

## How It Works

This application registers itself as a protocol handler for the `open://` protocol in the Windows Registry. When a link using this protocol is clicked (e.g., `open://C:/path/to/file.xlsx`), Windows launches this application and passes the URL as a command-line argument. The application then:

1. Parses the URL to extract the file or folder path
2. Determines whether the path is a file or folder
3. Opens the file with its associated application or opens the folder in Windows Explorer

## Requirements

- Windows operating system
- .NET 6.0 or higher
- Administrator rights (for registering the protocol handler)

## Installation

1. Build the application from source or download the released executable
2. Run the application **as administrator** (right-click → Run as administrator)
3. The application will register the protocol handler and display a success message
4. You only need to run as administrator once for the initial registration

## Usage

### In Web Pages

Add links to your web pages using the `open://` protocol:

```html
<a href="open://C:/path/to/file.xlsx">Open Excel File</a>
<a href="open://C:/path/to/folder">Open Folder</a>
```

### In Other Applications

You can use the protocol in any application that supports custom URL protocols:

```
open://C:/Users/username/Documents/important.pdf
```

### Testing

A test HTML file is included in the repository to verify that the protocol handler is working correctly. Open `Test.html` in a web browser after registering the handler to test both file and folder opening.

## Troubleshooting

If you encounter issues:

1. Ensure you ran the application as administrator for the initial registration
2. Verify that the file or folder path exists and is accessible
3. Check for error messages in the console window that appears when the handler is triggered
4. If using debug mode (default), the application will pause on errors to allow reading the messages

## Configuration

Edit the following constants in the source code before building:

- `PROTOCOL_NAME`: Change the protocol name (default is "open")
- `DEBUG_MODE`: Set to `true` for development and `false` for production use

## Security Considerations

- The protocol handler will attempt to open any file or folder path provided in the URL
- Users will see a warning from their browser before the protocol handler is launched
- Consider the security implications before deploying in production environments

## License

MIT

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
