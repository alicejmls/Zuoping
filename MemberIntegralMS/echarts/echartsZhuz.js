

//生成Echarts图形
function buildChart(legendData, axisLabel, seriesValue) {
    var chart = document.getElementById('barsDemo');
    var echart = echarts.init(chart);
    var option = {
        title: {
            text: "会员快速消费柱状图统计",//正标题
            x: "center", //标题水平方向位置
            y: "0", //标题竖直方向位置
            textStyle: {
                fontSize: 18,
                fontWeight: 'normal',
                color: '#666'
            }
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'//阴影，若需要为直线，则值为'line'
            }
        },
        toolbox: {
            show: true,
            feature: {
                saveAsImage: { show: true }
            }
        },
        legend: {
            data: legendData,
            y: 'bottom'//图例说明文字设置

        },
        grid: {
            left: '3%',
            right: '4%',
            bottom: '10%',
            top: '10%',
            containLabel: true
        },
        xAxis: [{
            min: 0,
            type: 'category', //纵向柱状图，若需要为横向，则此处值为'value'， 下面 yAxis 的type值为'category'
            data: axisLabel
        }],
        yAxis: [{
            min: 0,
            type: 'value',
            splitArea: { show: false }
        }],
        label: {
            normal: { //显示bar数据
                show: true,
                position: 'top'
            }
        },
        series: seriesValue
    };

    echart.setOption(option);
}


function dsagda(axisLabel, legendData) {


    var bgColorList = ['#FBB730', '#31BDF2', '#6197fb'];

    var seriesValue = [];

    var seriesDataVal = null;

    seriesDataVal = {
        barWidth: 10,//柱状条宽度
        name: '消费金额',
        type: 'bar',
        itemStyle: {
            normal: {
                show: true,//鼠标悬停时显示label数据
                barBorderRadius: [10, 10, 10, 10],//柱形图圆角，初始化效果
                color: bgColorList[1]
            }
        },
        label: {
            normal: {
                show: true, //显示数据
                position: 'top'//显示数据位置 'top/right/left/insideLeft/insideRight/insideTop/insideBottom'
            }
        },
        data: legendData
    };
    seriesValue.push(seriesDataVal);
    

    buildChart(legendData, axisLabel, seriesValue);
}

function initData() {


    var axisLabel = [];

    var legendData = [];

    $.ajax({
        type: 'get',
        url: '/StatisticalCenter/text',//请求数据的地址
        dataType: "json",        //返回数据形式为json
        success: function (result) {
            //请求成功时执行该函数内容，result即为服务器返回的json对象
            //'result.list' + index 请求json的其中一个
            //eval() 将对应的字符串解析成JS代码并运行
            $.each(eval(result), function (index, item) {
                axisLabel.push(item.MC_Name);
                legendData.push(item.CO_DiscountMoney);//挨个取出类别并填入类别数组


            });
            dsagda(axisLabel, legendData);
        },
        error: function (errorMsg) {
            //请求失败时执行该函数
            alert("图表请求数据失败!");
        }
    });






}