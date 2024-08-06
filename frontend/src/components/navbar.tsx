"use client"
import React from "react";
import { useAuth } from "../contexts/authContext";

const Navbar = () => {
    const {user, logout, email} = useAuth();
    
    return(
        <div className=" h-11 w-full bg-grape flex justify-between items-center">
            <div className="mx-2 flex flex-row w-full items-center">
                <div className="w-8 h-8 rounded-full bg-red-600 mr-2"></div>
                <span className="text-text">{email}</span>
            </div>
            <div className="mr-2">
                <button onClick={logout} className="text-text cursor-pointer hover:text-textHighlight">Logout</button>
            </div>
        </div>
    )

};

export default Navbar;