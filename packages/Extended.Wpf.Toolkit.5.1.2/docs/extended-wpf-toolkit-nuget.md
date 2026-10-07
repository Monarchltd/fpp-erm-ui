# Extended WPF Toolkit – Open-Source WPF UI Controls & Components for .NET

![Extended.Wpf.Toolkit](https://drive.google.com/uc?id=16YkRcsj5u8XFEw8x-ZMMboyuGmF6uYTR)

---

[![NuGet](https://img.shields.io/nuget/v/Extended.Wpf.Toolkit)](https://www.nuget.org/packages/Extended.Wpf.Toolkit)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Extended.Wpf.Toolkit)](https://www.nuget.org/packages/Extended.Wpf.Toolkit)
[![GitHub License](https://img.shields.io/github/license/xceedsoftware/wpftoolkit)](https://github.com/xceedsoftware/wpftoolkit/blob/master/license.md)

---

## Extended WPF Toolkit – Open-Source WPF Controls & Components for .NET

[![Product](https://img.shields.io/badge/Product-FF6F00?style=for-the-badge&logo=googlechrome&logoColor=white)](https://github.com/xceedsoftware/wpftoolkit)
[![Docs](https://img.shields.io/badge/Docs-FF6F00?style=for-the-badge&logo=readthedocs&logoColor=white)](https://github.com/xceedsoftware/wpftoolkit)
[![API Reference](https://img.shields.io/badge/API_Reference-FF6F00?style=for-the-badge&logo=html5&logoColor=white)](https://github.com/xceedsoftware/wpftoolkit)
[![Examples](https://img.shields.io/badge/Examples-FF6F00?style=for-the-badge&logo=github&logoColor=white)](https://github.com/xceedsoftware/wpftoolkit)
[![GitHub](https://img.shields.io/badge/GitHub-FF6F00?style=for-the-badge&logo=github&logoColor=white)](https://github.com/xceedsoftware/wpftoolkit)
[![Support](https://img.shields.io/badge/Support-FF6F00?style=for-the-badge&logo=googlemessages&logoColor=white)](https://github.com/xceedsoftware/wpftoolkit/issues)
[![License](https://img.shields.io/badge/License-FF6F00?style=for-the-badge&logo=rocket&logoColor=white)](https://github.com/xceedsoftware/wpftoolkit/blob/master/license.md)

---

## Overview

Extended WPF Toolkit is a free, open-source collection of WPF controls and components for .NET desktop applications. Built by Xceed Software and the community, it provides essential UI controls that extend the standard WPF framework with additional functionality, modern styling, and improved user experiences.

Ideal for developers building Windows desktop applications who want professional-grade controls without licensing costs. Includes DatePicker, TimePicker, NumericUpDown, PropertyGrid, CheckComboBox, and many more controls.

Community-driven and actively maintained on GitHub.

## Key Features

### Data & Input Controls
- DateTimePicker with custom formatting
- TimePicker for time selection
- NumericUpDown with validation
- MaskedTextBox for formatted input
- AutoCompleteBox for intelligent suggestions
- CheckListBox for multiple selections
- PropertyGrid for object editing
- MultiLineTextEditor

### Layout & Navigation
- Carousel for item carousel display
- BusyIndicator for loading states

### Visual & Interactive Components
- ColorPicker for color selection
- RichTextBox enhanced editor
- CheckComboBox for multi-select
- SplitButton and DropdownButton

### Styling & Theming
- MVVM-friendly architecture
- Full WPF template customization
- Custom styling support

### Developer Experience
- Open-source code on GitHub
- No licensing costs
- Community contributions
- Active development
- Comprehensive documentation
- Free to use and modify

## Installation

### NuGet Package Manager
```bash
dotnet add package Extended.Wpf.Toolkit
```

### Package Manager Console
```powershell
Install-Package Extended.Wpf.Toolkit
```

## Quick Start

### Use the DateTimePicker Control

```xml
<xctk:DateTimePicker
    x:Name="DatePicker"
    Value="{Binding SelectedDate}"
    Format="Custom"
    FormatString="MMMM dd, yyyy - hh:mm tt"
    Margin="10"/>
```

### Use the NumericUpDown Control

```xml
<xctk:IntegerUpDown
    Value="{Binding Quantity}"
    Minimum="0"
    Maximum="999"
    Margin="10"/>
```

### Create a PropertyGrid

```xml
<xctk:PropertyGrid
    SelectedObject="{Binding SelectedItem}"
    Margin="10"/>

public class Item
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}
```

## Core Capabilities

### Input Controls
| Feature | Supported |
|---------|-----------|
| DateTimePicker | ✓ |
| TimePicker | ✓ |
| NumericUpDown | ✓ |
| MaskedTextBox | ✓ |
| AutoCompleteBox | ✓ |
| CheckListBox | ✓ |
| CheckComboBox | ✓ |
| PropertyGrid | ✓ |

### Layout Controls
| Feature | Supported |
|---------|-----------|
| Carousel | ✓ |
| BusyIndicator | ✓ |

### Visual & Interactive
| Feature | Supported |
|---------|-----------|
| ColorPicker | ✓ |
| SplitButton | ✓ |
| DropdownButton | ✓ |
| RichTextBox | ✓ |

### Features
| Feature | Supported |
|---------|-----------|
| Open-source code | ✓ |
| MIT License | ✓ |
| No licensing costs | ✓ |
| Community maintained | ✓ |
| MVVM support | ✓ |
| Full source available | ✓ |

## Supported Frameworks

- .NET Framework 4.5+
- .NET Core 3.1
- .NET 5+
- .NET 6+
- .NET 7+
- .NET 8+
- .NET 9+
- .NET 10+

## Supported Platforms

- Windows Desktop (WPF)

## Common Use Cases

- **Open-Source Projects** - Build WPF apps without licensing concerns
- **Internal Business Tools** - Develop internal desktop applications
- **Educational Software** - Teach WPF and .NET development
- **Small Business Software** - Create cost-effective business solutions
- **Desktop Utilities** - Build Windows utility applications
- **Community Projects** - Contribute to open-source initiatives
- **Enterprise Applications** - Use as foundation for custom controls
- **Proof of Concept** - Prototype WPF applications quickly

## Resources

- **[GitHub Repository](https://github.com/xceedsoftware/wpftoolkit)** - Source code and documentation
- **[GitHub Issues](https://github.com/xceedsoftware/wpftoolkit/issues)** - Report bugs and request features
- **[NuGet Package](https://www.nuget.org/packages/Extended.Wpf.Toolkit)** - View on NuGet.org
- **[License](https://github.com/xceedsoftware/wpftoolkit/blob/master/license.md)** - MIT License details

## Related Xceed Products

### Enterprise WPF Components
- **[Xceed Toolkit Plus for WPF](https://www.nuget.org/packages/Xceed.Products.Wpf.Toolkit.Full)** - Commercial WPF controls with advanced features
- **[Xceed DataGrid for WPF](https://www.nuget.org/packages/Xceed.Products.Wpf.DataGrid.Full)** - High-performance DataGrid with virtualization

### Document & Office Automation
- **[Xceed Words for .NET](https://www.nuget.org/packages/Xceed.Words.NET)** - Word document creation and PDF conversion
- **[Xceed Workbooks for .NET](https://www.nuget.org/packages/Xceed.Workbooks.NET)** - Excel workbook creation and editing
- **[Xceed PDF Library for .NET](https://www.nuget.org/packages/Xceed.PdfLibrary.NET)** - PDF creation and manipulation

### File Transfer & Compression
- **[Xceed Zip for .NET](https://www.nuget.org/packages/Xceed.Products.Zip.Full)** - ZIP, TAR, GZip compression
- **[Xceed Real-Time Zip for .NET](https://www.nuget.org/packages/Xceed.Products.RealTimeZip.Full)** - Streaming ZIP compression
- **[Xceed SFTP for .NET](https://www.nuget.org/packages/Xceed.Products.SFtp.Full)** - Secure SFTP file transfer
- **[Xceed FTP for .NET](https://www.nuget.org/packages/Xceed.Products.Ftp.Full)** - FTP and FTPS file transfer

### Cross-Platform UI
- **[Xceed Toolkit for .NET MAUI](https://www.nuget.org/packages/Xceed.Product.Maui.Toolkit.Full)** - Cross-platform MAUI controls

### Lightweight Tools
- **[DocX](https://www.nuget.org/packages/DocX)** - Free Word document creation library

## Getting Help

- **GitHub Repository** - [Source code and issues](https://github.com/xceedsoftware/wpftoolkit)
- **GitHub Discussions** - [Community discussions and Q&A](https://github.com/xceedsoftware/wpftoolkit/discussions)
- **GitHub Issues** - [Report bugs and request features](https://github.com/xceedsoftware/wpftoolkit/issues)
- **NuGet Package** - [View on NuGet.org](https://www.nuget.org/packages/Extended.Wpf.Toolkit)

## License

Extended WPF Toolkit is free and open-source under the **MIT License**. You can use, modify, and distribute it freely in your projects. See the [license file](https://github.com/xceedsoftware/wpftoolkit/blob/master/license.md) for details.

---

[![GitHub](https://img.shields.io/badge/GitHub-FF6F00?style=for-the-badge&logo=github&logoColor=white)](https://github.com/xceedsoftware/wpftoolkit)
[![Issues](https://img.shields.io/badge/Issues-FF6F00?style=for-the-badge&logo=github&logoColor=white)](https://github.com/xceedsoftware/wpftoolkit/issues)
[![Discussions](https://img.shields.io/badge/Discussions-FF6F00?style=for-the-badge&logo=github&logoColor=white)](https://github.com/xceedsoftware/wpftoolkit/discussions)
[![License](https://img.shields.io/badge/License-MIT-FF6F00?style=for-the-badge)](https://github.com/xceedsoftware/wpftoolkit/blob/master/license.md)
[![NuGet](https://img.shields.io/badge/NuGet-FF6F00?style=for-the-badge&logo=nuget&logoColor=white)](https://www.nuget.org/packages/Extended.Wpf.Toolkit)

---

**Extended WPF Toolkit** – Free, open-source WPF controls for .NET desktop applications.
