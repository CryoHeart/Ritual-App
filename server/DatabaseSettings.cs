internal sealed record DatabaseSettings(
    string Host,
    int Port,
    string User,
    string Password,
    string Name,
    bool SslEnabled
);
