var createonp = document.getElementById("createonp").style;
var calculateequation = document.getElementById("calculateequation").style;
var calculateequationrange = document.getElementById("calculateequationrange").style;
var createequation = document.getElementById("createequation").style;
var home = document.getElementById("home").style;

createonp.display = "none";
calculateequation.display = "none"
calculateequationrange.display = "none"
createequation.display = "none"

function PageChanger(page)
{
    if(page >=0 && page <=5)
    {
        if(page == 0)
        {   home.display = "block";
            createonp.display = "none";
            calculateequation.display = "none";
            calculateequationrange.display = "none";
            createequation.display = "none";
        }
        if(page == 1)
        {
            home.display = "none";
            createonp.display = "block";
            calculateequation.display = "none";
            calculateequationrange.display = "none";
            createequation.display = "none";
        }
        if(page == 2)
        {
            home.display = "none";
            createonp.display = "none";
            calculateequation.display = "block";
            calculateequationrange.display = "none";
            createequation.display = "none";
        }
        if(page == 3)
        {
            home.display = "none";
            createonp.display = "none";
            calculateequation.display = "none";
            calculateequationrange.display = "block";
            createequation.display = "none";      
          
        }
        if(page == 4)
        {
            home.display = "none";
            createonp.display = "none";
            calculateequation.display = "none";
            calculateequationrange.display = "none";
            createequation.display = "block";
        }
        erase();
        TryConnection();
    }
    else
    {

    }
}

