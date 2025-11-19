-- 创建生日免费门票使用记录表
-- 用于记录用户每年使用生日免费门票的情况

CREATE TABLE IF NOT EXISTS BirthdayTickets (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    Year INTEGER NOT NULL,
    OrderNumber TEXT NOT NULL,
    UsedDate TEXT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Account(Id),
    UNIQUE(UserId, Year)
);

-- 创建索引以提高查询性能
CREATE INDEX IF NOT EXISTS idx_birthday_tickets_user_year ON BirthdayTickets(UserId, Year);

