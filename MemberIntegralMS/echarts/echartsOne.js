var myChart;
function echartStr(names, brower) {
    // 基于准备好的dom，初始化echarts实例
    if (myChart != null && myChart != "" && myChart != undefined) {
        myChart.dispose();
    }
    myChart = echarts.init(document.getElementById('main'));
    // 指定图表的配置项和数据
    var option = {
        title: {
            text: '会员快速消费金额统计',
            //subtext: '纯属虚构',
            x: 'center'
        },
        tooltip: {
            trigger: 'item',
            formatter: "{a} <br/>{b} : {c} ({d}%)"
        },
        legend: {
            orient: 'vertical',
            left: 'left',
            data: names
        },
        series: [
            {
                name: '会员消费',
                type: 'pie',
                radius: '55%',
                center: ['50%', '60%'],
                data: brower,
                itemStyle: {
                    emphasis: {
                        shadowBlur: 10,
                        shadowOffsetX: 0,
                        shadowColor: 'rgba(0, 0, 0, 0.5)'
                    }
                },
                label: {
                    normal: {
                        show: false,
                    }
                },
            }
        ]
    };

    // 使用刚指定的配置项和数据显示图表。
    myChart.setOption(option);
};

function qxfl() {
    var brower = [],
        names = [];
    //var index = $(that).data('index');


    $.ajax({
        type: 'get',
        url: '/StatisticalCenter/text',//请求数据的地址
        dataType: "json",        //返回数据形式为json
        success: function (result) {
            //请求成功时执行该函数内容，result即为服务器返回的json对象
            //'result.list' + index 请求json的其中一个
            //eval() 将对应的字符串解析成JS代码并运行
            $.each(eval(result), function (index, item) {

                names.push(item.MC_Name);    //挨个取出类别并填入类别数组 
                brower.push({
                    name: item.MC_Name,
                    value: item.CO_DiscountMoney
                });
            });
            echartStr(names, brower);
        },
        error: function (errorMsg) {
            //请求失败时执行该函数
            alert("图表请求数据失败!");
        }
    });
}

//缺陷分类


