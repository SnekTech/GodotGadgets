using FileAccess = System.IO.FileAccess;

namespace GodotGadgets;

public static class FileOperations
{
    public static async Task SafeWriteAllTextAsync(string path, string contents, CancellationToken ct = default)
    {
        // 1. 在同一目录下创建一个唯一的临时文件
        var tempPath = Path.Combine(Path.GetDirectoryName(path)!, $"$tmp_{Guid.NewGuid()}_{Path.GetFileName(path)}");

        try
        {
            // 2. 将所有内容写入临时文件
            await File.WriteAllTextAsync(tempPath, contents, ct);

            // 3. 确保数据完全写入磁盘
            await using (var fs = new FileStream(tempPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096,
                             true))
            {
                await fs.FlushAsync(ct);
            }

            // 4. 关键操作：用临时文件原子地替换目标文件
            // 此操作在大多数操作系统上是原子的
            File.Move(tempPath, path, overwrite: true);
        }
        finally
        {
            // 5. 清理工作：如果临时文件还存在（例如在替换前就被取消了），就删除它
            if (File.Exists(tempPath))
            {
                try
                {
                    File.Delete(tempPath);
                }
                catch
                {
                    /* 记录日志，但不要抛出异常 */
                }
            }
        }
    }
}