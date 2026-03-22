-- PostgreSQL folds unquoted identifiers to lowercase (books, id, title, author).

CREATE TABLE IF NOT EXISTS Books (
    Id SERIAL PRIMARY KEY,
    Title CHARACTER VARYING(500) NOT NULL,
    Author CHARACTER VARYING(200) NOT NULL
);

INSERT INTO Books (Title, Author)
VALUES
    ('The Pragmatic Programmer', 'David Thomas, Andrew Hunt'),
    ('Clean Code', 'Robert C. Martin');
