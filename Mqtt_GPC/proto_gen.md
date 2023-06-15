<ItemGroup>
    <PackageReference Include="Google.Protobuf" Version="3.23.3" />
    <PackageReference Include="Grpc.AspNetCore" Version="2.53.0" />
    <PackageReference Include="Grpc.Net.Client" Version="2.53.0" />
    <PackageReference Include="Grpc.Tools" Version="2.54.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>


  <ItemGroup>
		<Protobuf Include="exhook.proto" ProtoRoot="." GrpcServices="Server" OutputDir="./Mqtt_GPC" CompileOutputs="false" />
</ItemGroup>


