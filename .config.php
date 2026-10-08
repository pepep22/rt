<?php

$server = "localhost";
$port = 3306;
$user = "root";
$password = "";
$database = "clientes";

$conexion = mysqli_connect(
    $server,
    $user,
    $password,
    $database,
    $port
);

mysqli_query($conexion, $consulta);
?>