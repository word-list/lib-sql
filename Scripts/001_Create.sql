CREATE TABLE words (
    text STRING UNIQUE PRIMARY KEY,
    commonness INT,
    offensiveness INT,
    sentiment INT
);

CREATE TABLE word_types (
    name STRING UNIQUE PRIMARY KEY
);

CREATE TABLE word_word_types (
    word_text STRING REFERENCES words(text),
    word_type_name STRING REFERENCES word_types(name),
    PRIMARY KEY (word_text, word_type_name)
);