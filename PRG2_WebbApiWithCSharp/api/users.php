<?php

$dataFile = __DIR__ . '/users.json';

header("Content-Type: application/json");

$method = $_SERVER['REQUEST_METHOD'];

// hack för XAMP
if ($method === 'POST' && isset($_GET['_method'])) {
    $method = strtoupper($_GET['_method']);
}

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

        $found = false;
        foreach ($users as &$user) {
            if ($user['id'] == $id) {
                foreach ($input as $key => $value) {
                    if ($key !== 'id' && array_key_exists($key, $user)) {
                        $user[$key] = $value;
                    }
                }
                $found = true;
                break;
            }
        }

        if (!$found) {
            http_response_code(404);
            echo json_encode(["error" => "User not found"]);
            break;
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
