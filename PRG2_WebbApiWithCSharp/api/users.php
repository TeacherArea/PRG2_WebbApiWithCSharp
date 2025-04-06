<?php


echo $_SERVER['REQUEST_METHOD'];

$dataFile = __DIR__ . '/users.json';

header("Content-Type: application/json");

$method = $_SERVER['REQUEST_METHOD'];
$input = json_decode(file_get_contents('php://input'), true);
$id = $_GET['id'] ?? null;


$users = file_exists($dataFile) ? json_decode(file_get_contents($dataFile), true) : [];

switch ($method) {
    case 'GET':
        if ($id) {
            $user = array_filter($users, fn($u) => $u['id'] == $id);
            echo json_encode(array_values($user));
        } else {
            echo json_encode($users);
        }
        break;

    case 'POST':
        $newId = count($users) > 0 ? max(array_column($users, 'id')) + 1 : 1;
        $input['id'] = $newId;
        $users[] = $input;
        file_put_contents($dataFile, json_encode($users, JSON_PRETTY_PRINT));
        echo json_encode($input);
        break;

    case 'PUT':
        if (!$id) {
            http_response_code(400);
            echo json_encode(["error" => "Missing id"]);
            break;
        }
        foreach ($users as &$user) {
            if ($user['id'] == $id) {
                $user = array_merge($user, $input);
                break;
            }
        }
        file_put_contents($dataFile, json_encode($users, JSON_PRETTY_PRINT));
        echo json_encode(["status" => "updated"]);
        break;

    case 'DELETE':
        if (!$id) {
            http_response_code(400);
            echo json_encode(["error" => "Missing id"]);
            break;
        }
        $users = array_filter($users, fn($u) => $u['id'] != $id);
        file_put_contents($dataFile, json_encode(array_values($users), JSON_PRETTY_PRINT));
        echo json_encode(["status" => "deleted"]);
        break;

    default:
        http_response_code(405);
        echo json_encode(["error" => "Unsupported method"]);
        break;
}
?>
