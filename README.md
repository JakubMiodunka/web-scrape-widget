# web-scrape-widget

## Description

TODO

## Repository Structure

* */cfg* - Contains application configuration. Files placed there within the repository are just an example
of how the program can be configured. Feel free to edit those files according to our needs.
* */deb* - Directory, where program saves debug data such as logs or error reports.
* */doc* - Contains project documentation.
* */res* - Contains resources used by the program such as images, XML schemas etc.
* */src* - Stores *Visual Studio* projects containing source code of the program and unit tests.

## Documentation

To view project documentation containing more details about
program implementation please open */doc/html/index.html* file.

## Getting started

1. Clone the repository to Your machine.
2. Open the solution file placed in */src* directory using *Visual Studio* and build the project.
You can also run unit tests to make sure that everything works as expected.
3. Adjust program configuration placed in */cfg* directory according to Your needs.
For more details on how to do it, please refer to *Configuration* section of this *README*.
4. Launch the program and inspect, that Your configuration changes were applied successful.

## Configuration

Configuration of the program is stored in */cfg* directory.
It is divided into three main parts - general configuration of the app,
definitions of data sources used by the widget and interface layout defining how gathered data
shall be displayed within application window.

### General configuration

It is stored in */cfg/app_config.xml* file and points various places within file system,
from which program shall obtain required data.
Its structure is defined in */res/schemas/app_config_schema.xsd* XML schema - example of the file content below:

```XML
<?xml version="1.0" encoding="UTF-8"?>
<AppConfig>
    <ErrorReportsStorage Path="..\..\..\..\..\deb"/>
    <DataSourcesStorage Path="..\..\..\..\..\cfg\sources" RecursiveSearch="disabled"/>
    <InterfaceDefinition Path="..\..\..\..\..\cfg\interface_definition.xml"/>
</AppConfig>
```

WARNING: Path to this file is hardcoded within the code. File cannot be moved or renamed.

### Data sources definitions

Definitions of all data sources used by the application are placed in */cfg/sources* directory.
Data sources can be divided into couple of different types:
special data sources, website elements which program shall scrape and API calls.

#### Special data sources

Data sources available internally within the program - current there are two of those:

* *processor-usage* - Returns percentage value of current machine CPU usage.
* *ram-usage* - Returns percentage value of current machine RAM usage.

They can be referenced in interface definition directly.

#### Website elements

Data sources, which obtains their data from element of particular website pointed in their definition file.
Its structure is defined in */res/schemas/website_element_schema.xsd* XML schema - example of the file content below:

```XML
<?xml version="1.0" encoding="UTF-8"?>
<DataSource Name="nbp-pln-usd" DataUnit="" RefreshRate="PT1H">
    <Description>
        Current PLN/USD exchange rate published by National Bank of Poland.
    </Description>
    <WebsiteElement>
        <Website Url="https://nbp.pl/statystyka-i-sprawozdawczosc/kursy/tabela-a/"/>
        <HtmlNode XPath="/html/body/main/div/section/div/figure/table/tbody/tr[2]/td[3]"/>
        <NodeContentFilter Regex="\d*[,.]\d*"/>
    </WebsiteElement>
</DataSource>
```

Most of configuration parameters of this type of data source is self explanatory, but there are some,
which requires some additional comment:

* Data source *Description* - Description of data source available on application interface in entry tool tip prompt.
* *RefreshRate* property - Time interval of scraping new value from specified website element and updating application interface with it.
* *NodeContentFilter* - Regular expression, which first match from specified HTML node text content
shall be used as data obtained from particular data source.

#### API calls

Feature currently not available - planned to be introduced.

### Interface layout definition

Definition of application interface layout is placed by default in */cfg/interface_definition.xml* file.
Its structure is defined in */res/schemas/interface_definition_schema.xsd* XML schema - example of the file content below:

```XML
<?xml version="1.0" encoding="UTF-8"?>
<InterfaceDefinition>
    <Tab Name="PC Performance">
        <Section Name="Resources">
            <Entry Label="Processor Usage" DataSourceName="processor-usage"/>
        </Section>
    </Tab>
    <Tab Name="Economy">
        <Section Name="Currencies Exchange Rates">
            <Entry Label="PLN/USD" DataSourceName="nbp-pln-usd"/>
        </Section>
    </Tab>
</InterfaceDefinition>
```

Interface is fully configurable by the user and is divided into tabs, which contains sections and those contains entries.
Each entry shall specify, from which data source it shall obtain its content to display.

## Used Tools

* IDE: [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
* Documentation generator: [DoxyGen 1.12.0](https://www.doxygen.nl/)

## Authors

* Jakub Miodunka
  * [GitHub](https://github.com/JakubMiodunka)
  * [LinkedIn](https://www.linkedin.com/in/jakubmiodunka/)

## License

This project is licensed under the MIT License - see the *LICENSE.md* file for details.
