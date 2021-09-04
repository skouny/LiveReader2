<?php
// Static class
 class db {
    public static $mysqli;
    public static $query;
    public static function init() {
        // Connect to database
        self::$mysqli = new mysqli('localhost', 'myroot', 'zJp4$81s', 'livereader2');
        // If connection failed die...
        if (self::$mysqli->connect_errno) {
            die("Failed to connect to MySQL: (" . self::$mysqli->connect_errno . ') ' . self::$mysqli->connect_error);
        }
        // Change character set to utf8
        self::$mysqli->set_charset('utf8');
    }
    public static function executeNonQuery($sql) {
        // Execute the command
        self::$mysqli->query($sql);
        // If execution failed
        if (self::$mysqli->errno) {
            die("<pre>Failed to execute query!\n\nERROR (" . self::$mysqli->errno . "): " . self::$mysqli->error . "\n\nSQL COMMAND: {$sql}</pre>");
        }
        // Return last inserted id
        if (isset(self::$mysqli->insert_id)) {
            return self::$mysqli->insert_id;
        }
    }
    public static function executeQuery($sql) {
        self::$query = self::$mysqli->query($sql);
        // If execution failed
        if (self::$mysqli->errno) {
            die("<pre>Failed to execute query!\n\nERROR (" . self::$mysqli->errno . "): " . self::$mysqli->error . "\n\nSQL COMMAND: {$sql}</pre>");
        }
    }
    public static function freeQuery() {
        if (!empty(self::$query)) {
            self::$query->free();
        }
    }
    public static function getRecords($sql) {
        // Execute the command
        self::executeQuery($sql);
        $records = array();
        while ($record = self::$query->fetch_assoc()) {
            $records[] = $record;
        }
        // Free query set
        self::freeQuery();
        // Return
        return $records;
    }
    public static function getRecord($sql) {
        return self::getRecords($sql)[0];
    }
}
// Init
db::init();
// Compatibility
$mysqli = db::$mysqli;
