

//to switch between white star to yellow star uponclick



    let ratingGlobal = 0
    function RateStar(rating) {
        switch (rating) {
            case 1:

                document.getElementById("star1W").disabled = true;
                document.getElementById("star1Y").style.visibility = "visible";
                document.getElementById("star2W").disabled = true;
                document.getElementById("star2Y").style.visibility = "hidden";
                document.getElementById("star3W").disabled = true;
                document.getElementById("star3Y").style.visibility = "hidden";
                document.getElementById("star4W").disabled = true;
                document.getElementById("star4Y").style.visibility = "hidden";
                document.getElementById("star5W").disabled = true;
                document.getElementById("star5Y").style.visibility = "hidden";
                ratingGlobal = rating;

                break;

            case 2:

                document.getElementById("star1W").disabled = true;
                document.getElementById("star1Y").style.visibility = "visible";
                document.getElementById("star2W").disabled = true;
                document.getElementById("star2Y").style.visibility = "visible";
                document.getElementById("star3W").disabled = true;
                document.getElementById("star3Y").style.visibility = "hidden";
                document.getElementById("star4W").disabled = true;
                document.getElementById("star4Y").style.visibility = "hidden";
                document.getElementById("star5W").disabled = true;
                document.getElementById("star5Y").style.visibility = "hidden";
                ratingGlobal = rating;

                break;

            case 3:
                document.getElementById("star1W").disabled = true;
                document.getElementById("star1Y").style.visibility = "visible";
                document.getElementById("star2W").disabled = true;
                document.getElementById("star2Y").style.visibility = "visible";
                document.getElementById("star3W").disabled = true;
                document.getElementById("star3Y").style.visibility = "visible";
                document.getElementById("star4W").disabled = true;
                document.getElementById("star4Y").style.visibility = "hidden";
                document.getElementById("star5W").disabled = true;
                document.getElementById("star5Y").style.visibility = "hidden";
                ratingGlobal = rating;

                break;

            case 4:

                document.getElementById("star1W").disabled = true;
                document.getElementById("star1Y").style.visibility = "visible";
                document.getElementById("star2W").disabled = true;
                document.getElementById("star2Y").style.visibility = "visible";
                document.getElementById("star3W").disabled = true;
                document.getElementById("star3Y").style.visibility = "visible";
                document.getElementById("star4W").disabled = true;
                document.getElementById("star4Y").style.visibility = "visible";
                document.getElementById("star5W").disabled = true;
                document.getElementById("star5Y").style.visibility = "hidden";
                ratingGlobal = rating;

                break;

            case 5:

                document.getElementById("star1W").disabled = true;
                document.getElementById("star1Y").style.visibility = "visible";
                document.getElementById("star2W").disabled = true;
                document.getElementById("star2Y").style.visibility = "visible";
                document.getElementById("star3W").disabled = true;
                document.getElementById("star3Y").style.visibility = "visible";
                document.getElementById("star4W").disabled = true;
                document.getElementById("star4Y").style.visibility = "visible";
                document.getElementById("star5W").disabled = true;
                document.getElementById("star5Y").style.visibility = "visible";

                ratingGlobal = rating;
                break;
            default:
                break;
        }
    }
 function ValidateReview() {
        if (ratingGlobal == 0) {
            alert("Please rate the product");
            return false;
        }
        else {
            document.getElementById("ratingDummyInput").value = ratingGlobal;
            //alert(document.getElementById("ratingDummyInput").value)
            alert("Thank you for you review!");
            //return true;
        }
    }







