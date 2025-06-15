CREATE TABLE IF NOT EXISTS words (
    text STRING UNIQUE PRIMARY KEY,
    commonness INT,
    offensiveness INT,
    sentiment INT
);

CREATE TABLE IF NOT EXISTS word_types (
    name STRING UNIQUE PRIMARY KEY
);

CREATE TABLE IF NOT EXISTS word_word_types (
    word_text STRING REFERENCES words(text),
    word_type_name STRING REFERENCES word_types(name),
    PRIMARY KEY (word_text, word_type_name)
);