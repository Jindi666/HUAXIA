# 达梦数据库连接测试脚本
# 用于测试华夏基金大屏后端的达梦数据库连接和查询功能

$BaseUrl = "http://localhost:5000"
$TestUrl = "$BaseUrl/huaxia/screen/dashboard/testDmConnection"

Write-Host "================================================" -ForegroundColor Green
Write-Host "  华夏基金大屏后端 - 达梦数据库连接测试" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Green
Write-Host ""

Write-Host "测试目标: $TestUrl" -ForegroundColor Cyan
Write-Host "数据库类型: 达梦 DM8" -ForegroundColor Cyan
Write-Host "连接地址: localhost:5236" -ForegroundColor Cyan
Write-Host "数据库名称: HUAXIA" -ForegroundColor Cyan
Write-Host ""

try {
    Write-Host "正在测试达梦数据库连接..." -ForegroundColor Yellow

    # 发送POST请求测试数据库连接
    $response = Invoke-RestMethod -Uri $TestUrl -Method POST -ContentType "application/json" -Body "{}"

    Write-Host "✓ 数据库连接成功！" -ForegroundColor Green
    Write-Host ""
    Write-Host "连接信息:" -ForegroundColor Cyan
    Write-Host "  数据库类型: $($response.data.databaseType)" -ForegroundColor White
    Write-Host "  连接状态: $($response.data.connectionStatus)" -ForegroundColor White
    Write-Host "  测试结果: $($response.data.testResult)" -ForegroundColor White
    Write-Host "  消息: $($response.data.message)" -ForegroundColor White

} catch {
    Write-Host "✗ 数据库连接失败！" -ForegroundColor Red
    Write-Host ""
    Write-Host "错误详情:" -ForegroundColor Red
    Write-Host "  $($_.Exception.Message)" -ForegroundColor Red

    if ($_.ErrorDetails) {
        Write-Host ""
        Write-Host "服务器响应:" -ForegroundColor Red
        Write-Host "  $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "================================================" -ForegroundColor Green
Write-Host "  测试完成" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Green
