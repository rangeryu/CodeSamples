<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="AzurePaaSFTP" generation="1" functional="0" release="0" Id="09983827-4889-4b4e-8e9f-a450f24925b7" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="AzurePaaSFTPGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="FTPWorkerRole:FTP21" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/LB:FTPWorkerRole:FTP21" />
          </inToChannel>
        </inPort>
        <inPort name="FTPWorkerRole:FTP990" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/LB:FTPWorkerRole:FTP990" />
          </inToChannel>
        </inPort>
        <inPort name="FTPWorkerRole:FTP-Dyn-10000" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/LB:FTPWorkerRole:FTP-Dyn-10000" />
          </inToChannel>
        </inPort>
        <inPort name="FTPWorkerRole:FTP-Dyn-10001" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/LB:FTPWorkerRole:FTP-Dyn-10001" />
          </inToChannel>
        </inPort>
        <inPort name="FTPWorkerRole:FTP-Dyn-10002" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/LB:FTPWorkerRole:FTP-Dyn-10002" />
          </inToChannel>
        </inPort>
        <inPort name="FTPWorkerRole:FTP-Dyn-10003" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/LB:FTPWorkerRole:FTP-Dyn-10003" />
          </inToChannel>
        </inPort>
        <inPort name="FTPWorkerRole:FTP-Dyn-10004" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/LB:FTPWorkerRole:FTP-Dyn-10004" />
          </inToChannel>
        </inPort>
        <inPort name="FTPWorkerRole:FTP-Dyn-10005" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/LB:FTPWorkerRole:FTP-Dyn-10005" />
          </inToChannel>
        </inPort>
        <inPort name="FTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/LB:FTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Certificate|FTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" defaultValue="">
          <maps>
            <mapMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/MapCertificate|FTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
          </maps>
        </aCS>
        <aCS name="FTPWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/MapFTPWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="FTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="">
          <maps>
            <mapMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/MapFTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </maps>
        </aCS>
        <aCS name="FTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="">
          <maps>
            <mapMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/MapFTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </maps>
        </aCS>
        <aCS name="FTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="">
          <maps>
            <mapMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/MapFTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </maps>
        </aCS>
        <aCS name="FTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/MapFTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
          </maps>
        </aCS>
        <aCS name="FTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/MapFTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" />
          </maps>
        </aCS>
        <aCS name="FTPWorkerRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/MapFTPWorkerRoleInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:FTPWorkerRole:FTP21">
          <toPorts>
            <inPortMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole/FTP21" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:FTPWorkerRole:FTP990">
          <toPorts>
            <inPortMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole/FTP990" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:FTPWorkerRole:FTP-Dyn-10000">
          <toPorts>
            <inPortMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole/FTP-Dyn-10000" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:FTPWorkerRole:FTP-Dyn-10001">
          <toPorts>
            <inPortMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole/FTP-Dyn-10001" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:FTPWorkerRole:FTP-Dyn-10002">
          <toPorts>
            <inPortMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole/FTP-Dyn-10002" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:FTPWorkerRole:FTP-Dyn-10003">
          <toPorts>
            <inPortMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole/FTP-Dyn-10003" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:FTPWorkerRole:FTP-Dyn-10004">
          <toPorts>
            <inPortMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole/FTP-Dyn-10004" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:FTPWorkerRole:FTP-Dyn-10005">
          <toPorts>
            <inPortMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole/FTP-Dyn-10005" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:FTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput">
          <toPorts>
            <inPortMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole/Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </toPorts>
        </lBChannel>
        <sFSwitchChannel name="SW:FTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp">
          <toPorts>
            <inPortMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
          </toPorts>
        </sFSwitchChannel>
      </channels>
      <maps>
        <map name="MapCertificate|FTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" kind="Identity">
          <certificate>
            <certificateMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
          </certificate>
        </map>
        <map name="MapFTPWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapFTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" kind="Identity">
          <setting>
            <aCSMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </setting>
        </map>
        <map name="MapFTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" kind="Identity">
          <setting>
            <aCSMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </setting>
        </map>
        <map name="MapFTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" kind="Identity">
          <setting>
            <aCSMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </setting>
        </map>
        <map name="MapFTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
          </setting>
        </map>
        <map name="MapFTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole/Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" />
          </setting>
        </map>
        <map name="MapFTPWorkerRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRoleInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="FTPWorkerRole" generation="1" functional="0" release="0" software="F:\DevDevDev\RangerYuGitRepo\GithubCodeSamples\AzurePaaSFTP\AzurePaaSFTP\csx\Release\roles\FTPWorkerRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="FTP21" protocol="tcp" portRanges="21" />
              <inPort name="FTP990" protocol="tcp" portRanges="990" />
              <inPort name="FTP-Dyn-10000" protocol="tcp" portRanges="10000" />
              <inPort name="FTP-Dyn-10001" protocol="tcp" portRanges="10001" />
              <inPort name="FTP-Dyn-10002" protocol="tcp" portRanges="10002" />
              <inPort name="FTP-Dyn-10003" protocol="tcp" portRanges="10003" />
              <inPort name="FTP-Dyn-10004" protocol="tcp" portRanges="10004" />
              <inPort name="FTP-Dyn-10005" protocol="tcp" portRanges="10005" />
              <inPort name="Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" protocol="tcp" />
              <inPort name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp" portRanges="3389" />
              <outPort name="FTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/SW:FTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
                </outToChannel>
              </outPort>
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;FTPWorkerRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;FTPWorkerRole&quot;&gt;&lt;e name=&quot;FTP21&quot; /&gt;&lt;e name=&quot;FTP990&quot; /&gt;&lt;e name=&quot;FTP-Dyn-10000&quot; /&gt;&lt;e name=&quot;FTP-Dyn-10001&quot; /&gt;&lt;e name=&quot;FTP-Dyn-10002&quot; /&gt;&lt;e name=&quot;FTP-Dyn-10003&quot; /&gt;&lt;e name=&quot;FTP-Dyn-10004&quot; /&gt;&lt;e name=&quot;FTP-Dyn-10005&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
            <storedcertificates>
              <storedCertificate name="Stored0Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" certificateStore="My" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
                </certificate>
              </storedCertificate>
            </storedcertificates>
            <certificates>
              <certificate name="Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
            </certificates>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="FTPWorkerRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="FTPWorkerRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="FTPWorkerRoleInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="f5ce5ef4-7946-40a9-b516-695484f77ead" ref="Microsoft.RedDog.Contract\ServiceContract\AzurePaaSFTPContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="31cf3f5f-5a04-4787-8f4c-f6e7859a8988" ref="Microsoft.RedDog.Contract\Interface\FTPWorkerRole:FTP21@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole:FTP21" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="919084e7-1858-46ce-8aa0-3c3329a98acc" ref="Microsoft.RedDog.Contract\Interface\FTPWorkerRole:FTP990@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole:FTP990" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="41b2b0a0-0576-4a22-a6a3-61b353fb90c2" ref="Microsoft.RedDog.Contract\Interface\FTPWorkerRole:FTP-Dyn-10000@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole:FTP-Dyn-10000" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="cbe0ad58-5fc6-4990-a4f6-4ca66d185a26" ref="Microsoft.RedDog.Contract\Interface\FTPWorkerRole:FTP-Dyn-10001@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole:FTP-Dyn-10001" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="3d0d59f9-90ac-4c80-80b3-a8b23f87bbf9" ref="Microsoft.RedDog.Contract\Interface\FTPWorkerRole:FTP-Dyn-10002@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole:FTP-Dyn-10002" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="14213a08-b9e5-4837-9ff2-e47ccdc61c8a" ref="Microsoft.RedDog.Contract\Interface\FTPWorkerRole:FTP-Dyn-10003@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole:FTP-Dyn-10003" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="a9b99d54-8e59-411b-8bf6-c79a00ed4b72" ref="Microsoft.RedDog.Contract\Interface\FTPWorkerRole:FTP-Dyn-10004@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole:FTP-Dyn-10004" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="4379e870-1217-49d3-924f-c65d6663f504" ref="Microsoft.RedDog.Contract\Interface\FTPWorkerRole:FTP-Dyn-10005@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole:FTP-Dyn-10005" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="9f17d103-3210-4f8c-a69b-421e0ba7ac74" ref="Microsoft.RedDog.Contract\Interface\FTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/AzurePaaSFTP/AzurePaaSFTPGroup/FTPWorkerRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>