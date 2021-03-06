<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
  <?define ProductName="CSV Data Converter" ?>
  <?define ProductVersion="1.0.0.0" ?>
  <?define Manufacturer="Pecheroga"?>

  <Product Id="*" 
           Name="$(var.ProductName)" 
           Language="1033"
           Version="$(var.ProductVersion)"
           Manufacturer="$(var.Manufacturer)" 
           UpgradeCode="3fbc7ba8-6f5a-497c-bd6f-11f94d31c33f">
    <Package Id="*"
             Platform ="x86"
             InstallerVersion="200" 
             Compressed="yes" 
             InstallPrivileges="elevated"
             Manufacturer="$(var.Manufacturer)"            
             InstallScope="perMachine" />

    <MajorUpgrade DowngradeErrorMessage="A newer version of [ProductName] is already installed." />
    <MediaTemplate EmbedCab="yes" />

    <!--<UIRef Id="WixUI_InstallDir"/>-->
    
    <Property Id="INSTALLLOCATION" Value="ApplicationInstallDir"/>

    <Directory Id="TARGETDIR" Name="SourceDir" >
      <Directory Id="ProgramFilesFolder">
        <Directory Id="INSTALLLOCATION">
          <Component Id="RegistryProduct">
            <RegistryValue 
            Root="HKLM"
            Key ="SOFTWARE\$(var.Manufacturer)\$(var.ProductName)"
            Name="CurrentVersion"
            Value ="$(var.ProductVersion)"
            Type ="string"
            KeyPath ="yes"/>
          </Component>
          <Component Id="Converter"
                 Guid="{CD2901FF-F332-4575-BE03-36A769D18670}">
            <File Source="$(var.Converter.TargetPath)"
                  KeyPath="yes"/>
          </Component>
          <Component Id="App.config"
                     Guid="{0BC714E6-0B34-4F74-819F-C747BC5955DE}">
            <File Source="$(var.Converter.TargetPath).config"
                  KeyPath="yes"/>
          </Component>
          <Component Id="DataSource"
                     Guid="{D4D46308-10DB-40EC-A31B-A9624FB9AC6E}">
            <File Source="$(var.DataSource.TargetPath)"
                  KeyPath="yes"/>
          </Component>
          <Component Id="DataBase"
                     Guid="{578C567D-86D1-43AA-B486-152582CCF952}">
            <File Source="$(var.Converter.TargetDir)\ProgramsDB.mdf"
                  KeyPath="yes"
                  ReadOnly="no"/>
          </Component>
          <Component Id="DataBaseLog"
                     Guid="{6F39619F-DC96-4D3B-BC99-6433B709B4F6}">
            <File Source="$(var.Converter.TargetDir)\ProgramsDB_log.ldf"
                  KeyPath="yes"
                  ReadOnly="no"/>
          </Component>
          <Component Id="Interactivity"
                     Guid="{87B36EA7-30A1-43BF-A158-C8A320C7E054}">
            <File Source="$(var.Converter.TargetDir)\System.Windows.Interactivity.dll"
                  KeyPath="yes"/>
          </Component>
        </Directory>
      </Directory>
      
      <Directory Id="DesktopFolder">
      </Directory>

      <Directory Id="ProgramMenuFolder">
        <Directory Id="ShortcutFolder"
                   Name="$(var.ProductName)">
          <Component Id="ShortcutComponent"
                     Guid="{1A05E0EC-1CEC-4BFA-BFD8-4F06BD13F01B}"
                     Feature ="ProductFeature">
            <CreateFolder Directory="ShortcutFolder"/>
            <RegistryValue Id="RegistryShortcut"
                           Root="HKCU"
                           Key ="SOFTWARE\$(var.ProductName)\settings"
                           Name="Shortcut"
                           Value ="1"
                           Type ="integer"
                           KeyPath ="yes"/>
            <Shortcut Id="DesktopProductShorcut"
                      Name="$(var.ProductName)"
                      WorkingDirectory="INSTALLLOCATION"
                      Directory="DesktopFolder"
                      Target="[PRODUCT_SHORTCUT_TARGET]"
                      Icon="logo.ico"/>  
            <Shortcut Id="ProductShorcut"
                      Name="$(var.ProductName)"
                      WorkingDirectory="INSTALLLOCATION"
                      Directory="ShortcutFolder"
                      Target="[PRODUCT_SHORTCUT_TARGET]"
                      Icon="logo.ico"/>
            <Shortcut Id="UninstallProductShortcut"
                      Name="Uninstall $(var.ProductName)"
                      Target="[UNINSTALL_PRODUCT_SHORTCUT_TARGET]"
                      Arguments ="/uninstall"
                      Show="normal"/>
            <!--<Shortcut Id="UninstallProduct"
                  Name="Uninstall $(var.ProductName)"
                  Description="Uninstalls $(var.ProductName)"
                  Target="[System64Folder]msiexec.exe"
                  Arguments ="/x [ProductCode]"
                  Directory ="ShortcutFolder" />-->
            <RemoveFolder Id="RemoveShortcutFolder" On="uninstall"/>
          </Component>
        </Directory>
      </Directory>
    </Directory>
    
    <Feature Id="ProductFeature" Title="$(var.ProductName)" Level="1">
      <ComponentRef Id="RegistryProduct"/>
      <ComponentRef Id="Converter"/>
      <ComponentRef Id="App.config"/>
      <ComponentRef Id="DataSource"/>
      <ComponentRef Id="DataBase"/>
      <ComponentRef Id="DataBaseLog"/>
      <ComponentRef Id="Interactivity"/>
      <Feature Id="ShortcutFeature" Level="0">
        <ComponentRef Id="ShortcutComponent"/>  
      </Feature>
    </Feature>
    
    <!--<PropertyRef Id="NETFRAMEWORK45"/>
    <Condition Message="This setup requires .NET Framework 4.5 to be installed.">
      <![CDATA[Installed OR NETFRAMEWORK45]]>
    </Condition>

    <Condition Message="This setup requires SQL Server LocalDB to be installed.">
      <![CDATA[LocalDBVersion <= "11.0"]]>
    </Condition>

    <Property Id="LOCALDB">
      <RegistrySearch Id="SQLServerLocalDb" Root="HKLM" 
                      Key="SOFTWARE\Microsoft\Microsoft SQL Server\MSSQL11E.LOCALDB\MSSQLSERVER\CurrentVersion"  
                      Type="raw" Name="CurrentVersion" Win64="yes"/>
    </Property>
    <Condition Message ="This setup requires SQL Server LocalDB to be installed.">
      <![CDATA[Installed OR LOCALDB]]>
    </Condition>-->

    <!--<Property Id="EXCEL2003INSTALLED">
      <ComponentSearch Id="DetectExcel2003" Guid="{A2B280D4-20FB-4720-99F7-40C09FBCE10A}" Type="file"/>
    </Property>
    <Condition Message="This setup requires Microsoft Excel 2003 to be installed.">
      <![CDATA[Installed OR EXCEL2003INSTALLED]]>
    </Condition>-->

    <Icon Id="logo.ico" SourceFile ="$(var.Converter.ProjectDir)\logo.ico"/>
  </Product>
</Wix>
